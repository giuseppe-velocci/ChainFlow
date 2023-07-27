namespace ChainFlow.Models
{
    public record class ProcessingRequest
    {
        public ProcessingRequest(object request, ProcessingOutcome result)
        {
            Request = request;
            Result = result;
        }

        public object Request { get; }
        public ProcessingOutcome Result { get; }
    }
}