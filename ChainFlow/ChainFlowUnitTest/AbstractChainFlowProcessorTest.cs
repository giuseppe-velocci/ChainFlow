using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlow.Processors;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest
{
    public class AbstractChainFlowProcessorTest
    {
        private readonly Mock<AbstractChainFlowProcessor<bool>> Sut;
        private readonly Mock<IChainFlowBuilder> MockChainBuilder;

        public AbstractChainFlowProcessorTest()
        {
            MockChainBuilder = new ();
            Sut = new(MockChainBuilder.Object);
        }

        [Fact]
        public void Describe_WhenInvoked_ReturnsNull()
        {
            Sut.Object.Describe().Should().BeNull();
        }

        [Fact]
        public async Task ProcessAsync_WhenChainBuilderCannotResolveFlow_ThrowsException()
        {
            object message = new();
            var request = new ProcessingRequest(message);

            MockChainBuilder
                .Setup(x => x.Build())
                .Returns((IChainFlow)null!);

            var act = () => Sut.Object.ProcessAsync(request, CancellationToken.None);
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

            MockChainBuilder
                .Setup(x => x.Build())
                .Returns(mockFlow.Object);

            Sut
                .Setup(x => x.Outcome2T(response))
                .Returns(true);

            var result = await Sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().BeTrue();
        }
    }
}
