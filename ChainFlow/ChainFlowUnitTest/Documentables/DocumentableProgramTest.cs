using ChainFlow.Documentables;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChainFlowUnitTest.Documentables
{
    public class DocumentableProgramTest
    {
        private readonly DocumentableProgram _sut;
        private readonly Mock<IDocumentableWorkflow> _mockWorkflow0;
        private readonly Mock<IDocumentableWorkflow> _mockWorkflow1;
        private readonly Mock<ISystemIoWriter> _mockFilesystem;
        private readonly Mock<ILogger<DocumentableProgram>> _mockLogger;

        public DocumentableProgramTest()
        {
            _mockWorkflow0 = new();
            _mockWorkflow1 = new();
            _mockFilesystem = new();
            _mockLogger = new();
            _sut = new(
                new IDocumentableWorkflow[] { _mockWorkflow0.Object, _mockWorkflow1.Object },
                _mockFilesystem.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task RunAsync_WhenMultipleWorkflowsAreRegistred_CreatesDocuments()
        {
            string expected0 =
@"## name 0
describe 0

::: mermaid
graph TD;
_start(work 0) -->
flow 0
:::";

            string expected1 =
@"## name 1
describe 1

::: mermaid
graph TD;
_start(work 1) -->
flow 1
:::";

            SetupDocumentableWorkflow(_mockWorkflow0, "0");
            SetupDocumentableWorkflow(_mockWorkflow1, "1");

            await _sut.RunAsync();

            _mockFilesystem.Verify(x => x.WriteFile(It.IsAny<string>(), expected0), Times.Once);
            _mockFilesystem.Verify(x => x.WriteFile(It.IsAny<string>(), expected1), Times.Once);
        }

        [Fact]
        public async Task RunAsync_WhenSingleWorkflowIsRegistered_CreatesDocument()
        {
            string expected =
@"## name 0
describe 0

::: mermaid
graph TD;
_start(work 0) -->
flow 0
:::";
            SetupDocumentableWorkflow(_mockWorkflow0, "0");

            DocumentableProgram sut = new(
                new IDocumentableWorkflow[] { _mockWorkflow0.Object },
                _mockFilesystem.Object,
                _mockLogger.Object);

            await sut.RunAsync();

            _mockFilesystem.Verify(x => x.WriteFile("name_0.md", expected), Times.Once);
        }

        [Fact]
        public async Task RunAsync_WhenNullFilesystem_ThrowsException()
        {
            DocumentableProgram sut = new(new IDocumentableWorkflow[] { _mockWorkflow0.Object }, null!, _mockLogger.Object);

            var act = () => sut.RunAsync();
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task RunAsync_WhenNullWorkflows_ThrowsException()
        {
            DocumentableProgram sut = new(null!, _mockFilesystem.Object, _mockLogger.Object);

            var act = () => sut.RunAsync();
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task RunAsync_WhenEmptyWorkflows_NoFileIsCreated()
        {
            DocumentableProgram sut = new(Array.Empty<IDocumentableWorkflow>(), _mockFilesystem.Object, _mockLogger.Object);
            string expected = string.Empty;

            await sut.RunAsync();

            _mockFilesystem.Verify(x => x.WriteFile(It.IsAny<string>(), expected), Times.Never);
        }

        private static void SetupDocumentableWorkflow(Mock<IDocumentableWorkflow> mock, string id)
        {
            string name0 = "name " + id;
            mock
                .Setup(x => x.GetWorkflowName())
                .Returns(name0);
            string describe0 = "describe " + id;
            mock
                .Setup(x => x.Describe())
                .Returns(describe0);
            string work0 = "work " + id;
            mock
                .Setup(x => x.DescribeWorkflowEntryPoint())
                .Returns(work0);
            string flow0 = "flow " + id;
            mock
                .Setup(x => x.GetFlow())
                .Returns(flow0);
        }
    }
}
