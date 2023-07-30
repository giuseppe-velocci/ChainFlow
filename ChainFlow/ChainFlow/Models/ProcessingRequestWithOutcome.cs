using ChainFlow.Enums;

namespace ChainFlow.Models
{
    public record class ProcessingRequestWithOutcome : ProcessingRequest
    {
        public static ProcessingRequestWithOutcome CreateWithSuccess(object request, string message = "")
        {
            return new ProcessingRequestWithOutcome(request, FlowOutcome.Success, message);
        }

        public static ProcessingRequestWithOutcome CreateWithFailure(object request, string message)
        {
            return new ProcessingRequestWithOutcome(request, FlowOutcome.Failure, message);
        }
        
        public static ProcessingRequestWithOutcome CreateWithTransientFailure(object request, string message)
        {
            return new ProcessingRequestWithOutcome(request, FlowOutcome.TransientFailure, message);
        }

        internal ProcessingRequestWithOutcome(object request, FlowOutcome outcome, string message) : base(request)
        {
            Outcome = outcome;
            Message = message;
        }

        public FlowOutcome Outcome { get; }
        public string Message { get; }
    }
}