using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlow.Processors;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest
{
    public class AbstractChainFlowProcessorTest
    {
        private readonly Mock<AbstractChainFlowProcessor<bool>> _sut;
        private readonly Mock<IChainFlowBuilder> _mockChainBuilder;

        public AbstractChainFlowProcessorTest()
        {
            _mockChainBuilder = new ();
            _sut = new(_mockChainBuilder.Object);
        }

        [Fact]
        public void Describe_WhenInvoked_ReturnsNull()
        {
            _sut.Object.Describe().Should().BeNull();
        }

        [Fact]
        public async Task ProcessAsync_WhenChainBuilderCannotResolveFlow_ThrowsException()
        {
            object message = new();
            var request = new ProcessingRequest(message);

            _mockChainBuilder
                .Setup(x => x.Build())
                .Returns((IChainFlow)null!);

            var act = () => _sut.Object.ProcessAsync(request, CancellationToken.None);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task ProcessAsync_WhenChainBuilderResolvesFlow_RetrunsT()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingRequestWithOutcome.CreateWithSuccess(request);

            var mockFlow = new Mock<IChainFlow>();
            mockFlow.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _mockChainBuilder
                .Setup(x => x.Build())
                .Returns(mockFlow.Object);

            _sut
                .Setup(x => x.Outcome2T(response))
                .Returns(true);

            var result = await _sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().BeTrue();
        }
    }
}
