using ChainFlow.Models;

namespace ChainFlow.Interfaces.StorageFlowsDependencies
{
    public interface IStorageReader<TIn, TOut> where TIn : class where TOut : class
    {
        Task<OperationResult<TOut>> ReadAsync(TIn request, CancellationToken cancellationToken);
    }
}
