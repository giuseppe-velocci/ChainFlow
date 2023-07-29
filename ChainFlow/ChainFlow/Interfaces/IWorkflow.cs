using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IWorkflow<T> : IDocumentableWorkflow where T : notnull
    {
        Task<T> ExecuteAsync(ProcessingRequest message, CancellationToken cancellationToken);
    }
}
