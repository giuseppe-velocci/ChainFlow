namespace ChainFlow.Models
{
    public record class ProcessingRequest
    {
        public object Request { get; }

        public ProcessingRequest(object request)
        {
            Request = request;
        }
    }
}
