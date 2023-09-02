using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class BooleanRouterFlowTest
    {
        private readonly BooleanRouterFlow _sut;
        private readonly Mock<IRouterDispatcher<bool>> _mockDispatcher;

        public BooleanRouterFlowTest()
        {
            _mockDispatcher = new();
            _sut = new(_mockDispatcher.Object);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenDispatcherReturnsTrue_ReturnsRightFlow()
        {
            RequestToProcess request = new(new object());
            Mock<IChainFlow> _mockRightFlow = new();
            _sut.WithRightFlow(_mockRightFlow.Object);
            _mockDispatcher
                .Setup(x => x.ProcessAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _sut.ProcessRequestAsync(request, CancellationToken.None);
            _mockRightFlow.Verify(x => x.ProcessAsync(request, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenDispatcherReturnsFalse_ReturnsLeftFlow()
        {
            RequestToProcess request = new(new object());
            Mock<IChainFlow> _mockLeftFlow = new();
            _sut.WithLeftFlow(_mockLeftFlow.Object);
            _mockDispatcher
                .Setup(x => x.ProcessAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await _sut.ProcessRequestAsync(request, CancellationToken.None);
            _mockLeftFlow.Verify(x => x.ProcessAsync(request, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenDispatcherIsNull_ThrowsException()
        {
            RequestToProcess request = new(new object());
            BooleanRouterFlow sut = new(null!);

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ProcessRequestAsync_WhenNullFlowsAreDefind_ThrowsException(bool dispatchResult)
        {
            RequestToProcess request = new(new object());
            _mockDispatcher
                .Setup(x => x.ProcessAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dispatchResult);

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);
            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
