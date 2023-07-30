using ChainFlow.Enums;
using ChainFlow.Models;
using FluentAssertions;

namespace ChainFlowUnitTest.Models
{
    public class ProcessingRequestWithOutcomeTest
    {
        [Fact]
        public void CreateWithSuccess_WhenInvoked_RetunsFlowOutcomeSuccess()
        {
            object request = new();
            var sut = ProcessingRequestWithOutcome.CreateWithSuccess(request);

            sut.Request.Should().Be(request);
            sut.Message.Should().Be(string.Empty);
            sut.Outcome.Should().Be(FlowOutcome.Success);
        }
        [Fact]
        public void CreateWithSuccess_WhenInvokedWithMessage_RetunsFlowOutcomeSuccessAndMessage()
        {
            object request = new();
            string message = "Ok";
            var sut = ProcessingRequestWithOutcome.CreateWithSuccess(request, message);

            sut.Request.Should().Be(request);
            sut.Message.Should().Be(message);
            sut.Outcome.Should().Be(FlowOutcome.Success);
        }

        [Fact]
        public void CreateWithFailure_WhenInvoked_RetunsFlowOutcomeFailure()
        {
            object request = new();
            string message = "KO";
            var sut = ProcessingRequestWithOutcome.CreateWithFailure(request, message);

            sut.Request.Should().Be(request);
            sut.Message.Should().Be(message);
            sut.Outcome.Should().Be(FlowOutcome.Failure);
        }

        [Fact]
        public void CreateWithTransientFailure_WhenInvoked_RetunsFlowOutcomeTransientFailure()
        {
            object request = new();
            string message = "KO";
            var sut = ProcessingRequestWithOutcome.CreateWithTransientFailure(request, message);

            sut.Request.Should().Be(request);
            sut.Message.Should().Be(message);
            sut.Outcome.Should().Be(FlowOutcome.TransientFailure);
        }
    }
}
