using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IChainLink
    {
        Task<ProcessingRequest> ProcessAsync(ProcessingRequest message);
        void SetNext(IChainLink next);
        string Describe();
    }
}