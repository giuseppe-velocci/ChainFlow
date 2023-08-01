using ChainFlow.Enums;

namespace ChainFlow.Models
{
    public record class OperationResult<T>
    {

        internal OperationResult()
        {
            Value = default!;
            Message = default!;
        }

        private OperationResult(T value, FlowOutcome outcome, string message)
        {
            Value = value;
            Message = message;
            Outcome = outcome;
        }

        public T Value { get; }
        public string Message { get; }
        public FlowOutcome Outcome { get; }

        public static OperationResult<T> CreateWithSuccess(T value, string message = "") => 
            new (value, FlowOutcome.Success, message);
        
        public static OperationResult<T> CreateWithFailure(T value, string message) => 
            new (value, FlowOutcome.Failure, message); 

        public static OperationResult<T> CreateWithTransientFailure(T value, string message) => 
            new (value, FlowOutcome.TransientFailure, message);  

        public ProcessingResultWithOutcome ToProcessingRequestWithOutcome()
        {
            return new ProcessingResultWithOutcome(Value!, Outcome, Message);
        }
    }
}
