using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IWorkflow<T> : IDocumentableWorkflow where T : notnull
    {
        /// <summary>
        /// Entry point of request processing with inherited methods that allows auto documentation features
        /// </summary>
        /// <param name="message">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token to be propagated to all underlying flows</param>
        /// <returns></returns>
        Task<T> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);
    }
}
