using ChainFlow.Enums;
using ChainFlow.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainFlowUnitTest.Models
{
    public class OperationResultTest
    {
        [Fact]
        public void CreateWithSuccess_WhenInvoked_ReturnsFlowOutcomeSuccess()
        {
            object value = new object();
            OperationResult<object> sut = OperationResult<object>.CreateWithSuccess(value);

            sut.Value.Should().Be(value);
            sut.Message.Should().Be(string.Empty);
            sut.Outcome.Should().Be(FlowOutcome.Success);
        }

        [Fact]
        public void CreateWithSuccess_WhenInvokedWithMessage_ReturnsFlowOutcomeSuccess()
        {
            object value = new object();
            string message = "Ok";
            OperationResult<object> sut = OperationResult<object>.CreateWithSuccess(value, message);

            sut.Value.Should().Be(value);
            sut.Message.Should().Be(message);
            sut.Outcome.Should().Be(FlowOutcome.Success);
        }

        [Fact]
        public void CreateWithFailure_WhenInvoked_ReturnsFlowOutcomeFailure()
        {
            object value = new object();
            string message = "Ko";
            OperationResult<object> sut = OperationResult<object>.CreateWithFailure(value, message);

            sut.Value.Should().Be(value);
            sut.Message.Should().Be(message);
            sut.Outcome.Should().Be(FlowOutcome.Failure);
        }

        [Fact]
        public void CreateWithTransientFailure_WhenInvoked_ReturnsFlowOutcomeTransientFailure()
        {
            object value = new object();
            string message = "KO";
            OperationResult<object> sut = OperationResult<object>.CreateWithTransientFailure(value, message);

            sut.Value.Should().Be(value);
            sut.Message.Should().Be(message);
            sut.Outcome.Should().Be(FlowOutcome.TransientFailure);
        }

        [Theory]
        [MemberData(nameof(CreateOperationResult))]
        public void ToProcessingRequestWithOutcome_WhenInvoked_RetrunsMatching(
            OperationResult<object> operationResult,
            ProcessingRequestWithOutcome expected)
        {
            operationResult.Message.Should().Be(expected.Message);
            operationResult.Value.Should().Be(expected.Request);
            operationResult.Outcome.Should().Be(expected.Outcome);
        }

        public static IEnumerable<object[]> CreateOperationResult()
        {
            object value = new object() { };
            yield return new object[]
            {
                OperationResult<object>.CreateWithSuccess(value, "Ok"),
                ProcessingRequestWithOutcome.CreateWithSuccess(value, "Ok")
            };
            yield return new object[]
            {
                OperationResult<object>.CreateWithFailure(value, "Ko"),
                ProcessingRequestWithOutcome.CreateWithFailure(value, "Ko")
            };
            yield return new object[]
            {
                OperationResult<object>.CreateWithTransientFailure(value, "KO"),
                ProcessingRequestWithOutcome.CreateWithTransientFailure(value, "KO")
            };
        }
    }
}
