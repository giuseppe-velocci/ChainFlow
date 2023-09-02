using ChainFlow.ChainFlows.StorageFlows;
using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;
using ChainFlowUnitTest.Helpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class StorageWriterFlowTest
    {
        private readonly StorageWriterFlow<Input> _sut;
        private readonly Mock<IStorageWriter<Input>> _mockWriter;

        public StorageWriterFlowTest()
        {
            _mockWriter = new();
            _sut = new(_mockWriter.Object);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhennStorageSucceedsInput_RetunsInputData()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(true);
            _mockWriter
                .Setup(x => x.WriteAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingResult expected = ProcessingResult.CreateWithSuccess(input);

            ProcessingResult result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageFailsInput_RetunsFailure()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockWriter
                .Setup(x => x.WriteAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingResult expected = ProcessingResult.CreateWithFailure(input, operationResult.Message);

            ProcessingResult result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenStorageThrowsException_ThrowsException()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockWriter
                .Setup(x => x.WriteAsync(input, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidCastException());

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidCastException>();
        }

        [Fact]
        public async Task Ctor_WhenStorageIsNull_ThrowsException()
        {
            StorageWriterFlow<Input> sut = (null!);
            Input input = new("in");
            RequestToProcess request = new(input);
            var act = () => sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
