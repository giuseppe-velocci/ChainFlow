namespace ChainFlow.Models
{
    public sealed record class ProcessingRequestWithOutcome : ProcessingRequest
    {
        public static ProcessingRequestWithOutcome CreateWithSuccess(object request)
        {
            return new ProcessingRequestWithOutcome(request, FlowOutcome.Success, string.Empty);
        }

        public static ProcessingRequestWithOutcome CreateWithFailure(object request, string message)
        {
            return new ProcessingRequestWithOutcome(request, FlowOutcome.Failure, message);
        }
        
        public static ProcessingRequestWithOutcome CreateWithTransientFailure(object request, string message)
        {
            return new ProcessingRequestWithOutcome(request, FlowOutcome.TransientFailure, message);
        }

        public ProcessingRequestWithOutcome(object request, FlowOutcome outcome, string message) : base(request)
        {
            Outcome = outcome;
            OutcomeMessage = message;
        }

        public FlowOutcome Outcome { get; }
        public string OutcomeMessage { get; }
    }
}