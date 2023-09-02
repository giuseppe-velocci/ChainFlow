namespace ChainFlow.Models
{
    public record class RequestToProcess
    {
        public object Request { get; }

        public RequestToProcess(object request)
        {
            Request = request;
        }
    }
}
