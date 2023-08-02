using ChainFlow.Enums;

namespace ChainFlow.Models
{
    public record class ProcessingResultWithOutcome
    {
        public static ProcessingResultWithOutcome CreateWithSuccess(object result, string message = "")
        {
            return new ProcessingResultWithOutcome(result, FlowOutcome.Success, message);
        }

        public static ProcessingResultWithOutcome CreateWithFailure(object result, string message)
        {
            return new ProcessingResultWithOutcome(result, FlowOutcome.Failure, message);
        }

        public static ProcessingResultWithOutcome CreateWithTransientFailure(object result, string message)
        {
            return new ProcessingResultWithOutcome(result, FlowOutcome.TransientFailure, message);
        }

        internal ProcessingResultWithOutcome(object result, FlowOutcome outcome, string message)
        {
            Outcome = outcome;
            Message = message;
            Result = result;
        }

        public FlowOutcome Outcome { get; }
        public string Message { get; }
        public object Result { get; }
    }
}