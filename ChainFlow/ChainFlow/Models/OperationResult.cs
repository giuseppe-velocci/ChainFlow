﻿using ChainFlow.Enums;

namespace ChainFlow.Models
{
    public record class OperationResult<T>
    {
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
            new OperationResult<T>(value, FlowOutcome.Success, message);
        
        public static OperationResult<T> CreateWithFailure(T value, string message) => 
            new OperationResult<T>(value, FlowOutcome.Failure, message); 

        public static OperationResult<T> CreateWithTransientFailure(T value, string message) => 
            new OperationResult<T>(value, FlowOutcome.TransientFailure, message);  

        public ProcessingRequestWithOutcome ToProcessingRequestWithOutcome()
        {
            return new ProcessingRequestWithOutcome(Value!, Outcome, Message);
        }
    }
}