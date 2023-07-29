using ChainFlow.Documentables;
using ChainFlow.Interfaces;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.Documentables
{
    public class DocumentableProgramTest
    {
        private readonly DocumentableProgram _sut;
        private readonly Mock<IDocumentableWorkflow> _mockWorkflow0;
        private readonly Mock<IDocumentableWorkflow> _mockWorkflow1;
        private readonly Mock<ISystemIoWriter> _mockFilesystem;

        public DocumentableProgramTest()
        {
            _mockWorkflow0 = new();
            _mockWorkflow1 = new();
            _mockFilesystem = new();
            _sut = new(new IDocumentableWorkflow[] { _mockWorkflow0.Object, _mockWorkflow1.Object }, _mockFilesystem.Object);
        }

        [Fact]
        public async Task RunAsync_WhenMultipleWorkflowsAreRegistred_CreatesDocument()
        {
            string expected =
@"##name 0
describe 0

::: mermaid
graph TD;
work 0
flow 0
:::

##name 1
describe 1

::: mermaid
graph TD;
work 1
flow 1
:::";

            SetupDocumentableWorkflow(_mockWorkflow0, "0");
            SetupDocumentableWorkflow(_mockWorkflow1, "1");

            await _sut.RunAsync(Array.Empty<string>());

            _mockFilesystem.Verify(x => x.WriteFile(It.IsAny<string>(), expected), Times.Once);
        }

        [Fact]
        public async Task RunAsync_WhenSingleWorkflowIsRegistered_CreatesDocument()
        {
            string expected =
@"##name 0
describe 0

::: mermaid
graph TD;
work 0
flow 0
:::";
            SetupDocumentableWorkflow(_mockWorkflow0, "0");

            DocumentableProgram sut = new(new IDocumentableWorkflow[] { _mockWorkflow0.Object }, _mockFilesystem.Object);

            await sut.RunAsync(Array.Empty<string>());

            _mockFilesystem.Verify(x => x.WriteFile("name_0.md", expected), Times.Once);
        }

        [Fact]
        public async Task RunAsync_WhenNullFilesystem_ThrowsException()
        {
            DocumentableProgram sut = new(Array.Empty<IDocumentableWorkflow>(), null!);

            var act = () => sut.RunAsync(Array.Empty<string>());
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task RunAsync_WhenNullWorkflows_ThrowsException()
        {
            DocumentableProgram sut = new(null!, _mockFilesystem.Object);

            var act = () => sut.RunAsync(Array.Empty<string>());
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task RunAsync_WhenEmptyWorkflows_returnsEmptyString()
        {
            DocumentableProgram sut = new(Array.Empty<IDocumentableWorkflow>(), _mockFilesystem.Object);
            string expected = string.Empty;

            await sut.RunAsync(Array.Empty<string>());

            _mockFilesystem.Verify(x => x.WriteFile(It.IsAny<string>(), expected), Times.Once);
        }

        private void SetupDocumentableWorkflow(Mock<IDocumentableWorkflow> mock, string id)
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
