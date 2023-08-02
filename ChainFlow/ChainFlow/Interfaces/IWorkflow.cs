using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IWorkflow : IDocumentableWorkflow
    {
        /// <summary>
        /// Entry point of request processing with inherited methods that allows auto documentation features
        /// </summary>
        /// <param name="message">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token to be propagated to all underlying flows</param>
        /// <returns>ProcessingRequestWithOutcome</returns>
        Task<ProcessingResultWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);
    }
}
