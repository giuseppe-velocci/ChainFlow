namespace ChainFlow.Models
{
    public record class ProcessingOutcome
    {
        public ProcessingOutcome(FlowOutcome flowOutcome, string outcomeMessage)
        {
            FlowOutcome = flowOutcome;
            OutcomeMessage = outcomeMessage;
        }

        public FlowOutcome FlowOutcome { get; }
        public string OutcomeMessage { get; }
    }
}