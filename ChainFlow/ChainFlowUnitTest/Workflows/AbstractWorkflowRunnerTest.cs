using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlow.Workflows;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.Workflows
{
    public class AbstractWorkflowRunnerTest
    {
        private readonly Mock<AbstractWorkflowRunner> _sut;
        private readonly Mock<IChainFlowBuilder> _mockChainBuilder;

        public AbstractWorkflowRunnerTest()
        {
            _mockChainBuilder = new();
            _sut = new(_mockChainBuilder.Object);
        }

        [Fact]
        public async Task ProcessAsync_WhenChainBuilderCannotResolveFlow_ThrowsException()
        {
            object message = new();
            var request = new ProcessingRequest(message);

            _mockChainBuilder
                .Setup(x => x.Build(FlowOutcome.Success))
                .Returns((IChainFlow)null!);

            var act = () => _sut.Object.ProcessAsync(request, CancellationToken.None);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task ProcessAsync_WhenChainBuilderResolvesFlow_RetrunsProcessingRequestWithOutcome()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingResultWithOutcome.CreateWithSuccess(request);

            var mockFlow = new Mock<IChainFlow>();
            mockFlow.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _mockChainBuilder
                .Setup(x => x.Build(FlowOutcome.Success))
                .Returns(mockFlow.Object);

            var result = await _sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public void GetFlow_WhenIChainBuilderIsNotNull_ReturnsIChainBuilderToString()
        {
            string expected = "a flow";
            _mockChainBuilder.Setup(x => x.ToString())
                .Returns(expected);

            string result = _sut.Object.GetFlow();

            result.Should().Be(expected);
        }
    }
}
