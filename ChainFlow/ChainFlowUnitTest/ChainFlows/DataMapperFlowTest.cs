using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.Interfaces.DataFlowsDependencies;
using ChainFlow.Models;
using ChainFlowUnitTest.Helpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class DataMapperFlowTest
    {
        private readonly DataMapperFlow<Input, Output> _sut;
        private readonly Mock<IDataMapper<Input, Output>> _mockMapper;

        public DataMapperFlowTest()
        {
            _mockMapper = new();
            _sut = new(_mockMapper.Object);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenMappingSucceedsInput_RetunsMappedData()
        {
            Input input = new("in");
            Output output = new("out");
            ProcessingRequest request = new(input);
            var expected = OperationResult<Output>.CreateWithSuccess(output);
            _mockMapper
                .Setup(x => x.MapAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            ProcessingResultWithOutcome result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Outcome.Should().Be(expected.Outcome);
            result.Message.Should().Be(expected.Message);
            result.Result.Should().Be(expected.Value);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenMappingFailsInput_RetunsFailure()
        {
            Input input = new("in");
            Output output = null!;
            ProcessingRequest request = new(input);
            var expected = OperationResult<Output>.CreateWithFailure(output, "Ko");
            _mockMapper
                .Setup(x => x.MapAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            ProcessingResultWithOutcome result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Outcome.Should().Be(expected.Outcome);
            result.Message.Should().Be(expected.Message);
            result.Result.Should().Be(expected.Value);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenMapperThrowsException_ThrowsException()
        {
            Input input = new("in");
            ProcessingRequest request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockMapper
                .Setup(x => x.MapAsync(input, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidCastException());

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidCastException>();
        }
        [Fact]
        public async Task Ctor_WhenMapperIsNull_ThrowsException()
        {
            DataMapperFlow<Input, Output> sut = (null!);
            Input input = new("in");
            ProcessingRequest request = new(input);
            var act = () => sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
