namespace ChainFlow.Interfaces
{
    public interface IChainLink
    {
        Task<ProcessingMessage> ProcessAsync(ProcessingMessage message);
        void SetNext(IChainLink next);
        string Describe();
    }
}