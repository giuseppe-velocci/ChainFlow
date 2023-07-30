using ChainFlow.Models;

namespace ChainFlow.Interfaces.StorageFlowsDependencies
{
    public interface IStorageRemover<in TIn> where TIn : class
    {
        Task<OperationResult<bool>> RemoveAsync(TIn request, CancellationToken cancellationToken);
    }
}
