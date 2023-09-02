using ChainFlow.ChainFlows.StorageFlows;
using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;
using ChainFlowUnitTest.Helpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class StorageReaderFlowTest
    {
        private readonly StorageReaderFlow<Input, Output> _sut;
        private readonly Mock<IStorageReader<Input, Output>> _mockRemover;

        public StorageReaderFlowTest()
        {
            _mockRemover = new();
            _sut = new(_mockRemover.Object);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageSucceedsInput_RetunReadData()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            Output output = new("out");
            var operationResult = OperationResult<Output>.CreateWithSuccess(output);
            _mockRemover
                .Setup(x => x.ReadAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingResult expected = ProcessingResult.CreateWithSuccess(output);

            ProcessingResult result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageFailsInput_RetunsFailure()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            Output output = new("out");
            var operationResult = OperationResult<Output>.CreateWithFailure(output, "Ko");
            _mockRemover
                .Setup(x => x.ReadAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingResult expected = ProcessingResult.CreateWithFailure(output, operationResult.Message);

            ProcessingResult result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageThrowsException_ThrowsException()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            Output output = new("out");
            var operationResult = OperationResult<Output>.CreateWithSuccess(output);
            _mockRemover
                .Setup(x => x.ReadAsync(input, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidCastException());

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidCastException>();
        }

        [Fact]
        public async Task Ctor_WhenStorageIsNull_ThrowsException()
        {
            StorageReaderFlow<Input, Output> sut = (null!);
            Input input = new("in");
            RequestToProcess request = new(input);
            var act = () => sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
