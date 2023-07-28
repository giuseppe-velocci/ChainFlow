using ChainFlow.Models;

namespace ChainFlow.Interfaces
{
    public interface IRouterLogic<T> where T : notnull
    {
        Task<T> ExcecuteAsync(ProcessingRequest message, CancellationToken cancellationToken);
        string Describe();
    }
}