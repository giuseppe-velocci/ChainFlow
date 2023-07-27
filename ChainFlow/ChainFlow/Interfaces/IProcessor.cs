using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IProcessor<T> : IDocumentableWorkflow where T : notnull
    {
        Task<T> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);
    }
}
