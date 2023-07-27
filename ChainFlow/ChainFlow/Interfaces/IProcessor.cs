using ChainFlow.Documentables;
using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IProcessor<T> : IDocumentableFlow where T : notnull
    {
        Task<T> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);
    }
}
