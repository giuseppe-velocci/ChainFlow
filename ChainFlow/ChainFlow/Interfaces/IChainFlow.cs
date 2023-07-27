using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IChainFlow
    {
        Task<ProcessingRequestWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);
        void SetNext(IChainFlow next);
        string Describe();
    }
}