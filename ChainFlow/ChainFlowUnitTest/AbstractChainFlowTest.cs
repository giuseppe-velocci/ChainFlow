using ChainFlow.ChainLinks;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest
{
    public class AbstractChainFlowTest
    {
        private readonly Mock<AbstractChainFlow> Sut;

        public AbstractChainFlowTest()
        {
            Sut = new();
        }

        [Fact]
        public void Describe_WhenInvoked_ReturnsNull()
        {
            Sut.Object.Describe().Should().BeNull();
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsNotSet_ReturnsConcreteLinkResult()
        {
            object message = new();
            var request = ProcessingRequestWithOutcome.CreateWithSuccess(message);
            var response = ProcessingRequestWithOutcome.CreateWithSuccess(message);
            Sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequestWithOutcome>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await Sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsSetAndProcessingSucceeds_ReturnsNextLinkResult()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingRequestWithOutcome.CreateWithFailure(message, string.Empty);
            Sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ProcessingRequestWithOutcome.CreateWithSuccess(message));

            Mock<IChainFlow> next = new();
            next.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Sut.Object.SetNext(next.Object);

            var result = await Sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsSetAndProcessingFails_ReturnsNextLinkResult()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingRequestWithOutcome.CreateWithFailure(message, string.Empty);
            Sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Mock<IChainFlow> next = new();
            next.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ProcessingRequestWithOutcome.CreateWithSuccess(message));

            Sut.Object.SetNext(next.Object);

            var result = await Sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsSetAndProcessingExitsWithTransientFailure_ReturnsNextLinkResult()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingRequestWithOutcome.CreateWithTransientFailure(message, string.Empty);
            Sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Mock<IChainFlow> next = new();
            next.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ProcessingRequestWithOutcome.CreateWithSuccess(message));

            Sut.Object.SetNext(next.Object);

            var result = await Sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenProcessingThrowsException_ThrowsException()
        {
            object message = new();
            var request = new ProcessingRequest(message);

            Sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new IndexOutOfRangeException());

            var act = () => Sut.Object.ProcessAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<IndexOutOfRangeException>();
        }
    }
}
