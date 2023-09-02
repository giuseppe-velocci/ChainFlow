using ChainFlow.Enums;

namespace ChainFlow.Models
{
    public record class ProcessingResult
    {
        public static ProcessingResult CreateWithSuccess(object result, string message = "")
        {
            return new ProcessingResult(result, FlowOutcome.Success, message);
        }

        public static ProcessingResult CreateWithFailure(object result, string message)
        {
            return new ProcessingResult(result, FlowOutcome.Failure, message);
        }

        public static ProcessingResult CreateWithTransientFailure(object result, string message)
        {
            return new ProcessingResult(result, FlowOutcome.TransientFailure, message);
        }

        internal ProcessingResult(object result, FlowOutcome outcome, string message)
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