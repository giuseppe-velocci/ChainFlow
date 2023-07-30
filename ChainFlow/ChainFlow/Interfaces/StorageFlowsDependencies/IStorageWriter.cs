using ChainFlow.Models;

namespace ChainFlow.Interfaces.StorageFlowsDependencies
{
    public interface IStorageWriter<in TIn> where TIn : class
    {
        Task<OperationResult<bool>> WriteAsync(TIn request, CancellationToken cancellationToken);
    }
}
