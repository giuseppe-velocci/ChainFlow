using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class AbstractChainFlowTest
    {
        private readonly Mock<AbstractChainFlow> _sut;

        public AbstractChainFlowTest()
        {
            _sut = new();
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsNotSet_ReturnsConcreteLinkResult()
        {
            object message = new();
            var request = ProcessingResultWithOutcome.CreateWithSuccess(message);
            var response = ProcessingResultWithOutcome.CreateWithSuccess(message);
            _sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Object.ProcessAsync(new ProcessingRequest(request.Result), CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsSetAndProcessingSucceeds_ReturnsNextLinkResult()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingResultWithOutcome.CreateWithFailure(message, string.Empty);
            _sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ProcessingResultWithOutcome.CreateWithSuccess(message));

            Mock<IChainFlow> next = new();
            next.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _sut.Object.SetNext(next.Object);

            var result = await _sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsSetAndProcessingFails_ReturnsNextLinkResult()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingResultWithOutcome.CreateWithFailure(message, string.Empty);
            _sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Mock<IChainFlow> next = new();
            next.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ProcessingResultWithOutcome.CreateWithSuccess(message));

            _sut.Object.SetNext(next.Object);

            var result = await _sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenNextChainLinkIsSetAndProcessingExitsWithTransientFailure_ReturnsNextLinkResult()
        {
            object message = new();
            var request = new ProcessingRequest(message);
            var response = ProcessingResultWithOutcome.CreateWithTransientFailure(message, string.Empty);
            _sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Mock<IChainFlow> next = new();
            next.Setup(x => x.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ProcessingResultWithOutcome.CreateWithSuccess(message));

            _sut.Object.SetNext(next.Object);

            var result = await _sut.Object.ProcessAsync(request, CancellationToken.None);
            result.Should().Be(response);
        }

        [Fact]
        public async Task ProcessAsync_WhenProcessingThrowsException_ThrowsException()
        {
            object message = new();
            var request = new ProcessingRequest(message);

            _sut
                .Setup(x => x.ProcessRequestAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new IndexOutOfRangeException());

            var act = () => _sut.Object.ProcessAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<IndexOutOfRangeException>();
        }
    }
}
