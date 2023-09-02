using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.Interfaces.DataFlowsDependencies;
using ChainFlow.Models;
using ChainFlowUnitTest.Helpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainFlows
{
    public class DataValidatorFlowTest
    {
        private readonly DataValidatorFlow<Input> _sut;
        private readonly Mock<IDataValidator<Input>> _mockValidator;

        public DataValidatorFlowTest()
        {
            _mockValidator = new();
            _sut = new(_mockValidator.Object);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenValidationSucceedsInput_RetunsInputData()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(true);
            _mockValidator
                .Setup(x => x.ValidateAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingResult expected = ProcessingResult.CreateWithSuccess(input);

            ProcessingResult result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenValidationFailsInput_RetunsFailure()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockValidator
                .Setup(x => x.ValidateAsync(input, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);
            ProcessingResult expected = ProcessingResult.CreateWithFailure(input, operationResult.Message);

            ProcessingResult result = await _sut.ProcessRequestAsync(request, CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ProcessRequestAsync_WhenValidatorThrowsException_ThrowsException()
        {
            Input input = new("in");
            RequestToProcess request = new(input);
            var operationResult = OperationResult<bool>.CreateWithSuccess(false);
            _mockValidator
                .Setup(x => x.ValidateAsync(input, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidCastException());

            var act = () => _sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidCastException>();
        }

        [Fact]
        public async Task Ctor_WhenValidatorIsNull_ThrowsException()
        {
            DataValidatorFlow<Input> sut = (null!);
            Input input = new("in");
            RequestToProcess request = new(input);
            var act = () => sut.ProcessRequestAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
