using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IChainFlow
    {
        /// <summary>
        /// Execute defined logic on incoming request returning a result with the original or modified input
        /// </summary>
        /// <param name="message">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>ProcessingRequest with meta information abut the outcome of processing</returns>
        Task<ProcessingRequestWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);

        /// <summary>
        /// Define the flow that will get to process input data
        /// </summary>
        /// <param name="next">Next IChainFlow in the workflow</param>
        void SetNext(IChainFlow next);

        /// <summary>
        /// Decsription of the routing logic for documentation
        /// </summary>
        /// <returns>String description of current logic for documentation</returns>
        string Describe();
    }
}