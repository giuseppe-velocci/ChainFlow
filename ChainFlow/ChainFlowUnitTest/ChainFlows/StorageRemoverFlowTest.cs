using ChainFlow.ChainFlows.StorageFlows;
using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;
using ChainFlowUnitTest.Helpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class StorageRemoverFlowTest
    {
        private readonly StorgeRemoverFlow<Input> _sut;
        private readonly Mock<IStorageRemover<Input>> _mockRemover;

        public StorageRemoverFlowTest()
        {
            _mockRemover = new();
            _sut = new(_mockRemover.Object);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhennStorageSucceedsInput_RetunsInputData()
        {
            Input input = new("in");
            ProcessingRequest request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(true);
            _mockRemover
                .Setup(x => x.RemoveAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingRequestWithOutcome expected = ProcessingRequestWithOutcome.CreateWithSuccess(input);

            ProcessingRequestWithOutcome result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageFailsInput_RetunsFailure()
        {
            Input input = new("in");
            ProcessingRequest request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockRemover
                .Setup(x => x.RemoveAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingRequestWithOutcome expected = ProcessingRequestWithOutcome.CreateWithFailure(input, operationResult.Message);

            ProcessingRequestWithOutcome result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageThrowsException_ThrowsException()
        {
            Input input = new("in");
            ProcessingRequest request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockRemover
                .Setup(x => x.RemoveAsync(input, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidCastException());

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidCastException>();
        }

        [Fact]
        public async Task Ctor_WhenStorageIsNull_ThrowsException()
        {
            StorgeRemoverFlow<Input> sut = (null!);
            Input input = new("in");
            ProcessingRequest request = new(input);
            var act = () => sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
