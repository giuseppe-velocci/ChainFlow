using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IRouterDispatcher<T> where T : notnull
    {
        /// <summary>
        /// Process incoming request and returns a T value that will be used by RouterFlow to route the request to the correct flow
        /// </summary>
        /// <param name="message">Input received by RouterFlow</param>
        /// <param name="cancellationToken">Cancellation token received by RouterFlow</param>
        /// <returns>T, result of the dispatching logic implemented by the concrete of this interface</returns>
        Task<T> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);

        /// <summary>
        /// Decsription of the routing logic for documentation. It should be defined as a question
        /// </summary>
        /// <returns>String description of current logic for documentation</returns>
        string Describe();
    }
}