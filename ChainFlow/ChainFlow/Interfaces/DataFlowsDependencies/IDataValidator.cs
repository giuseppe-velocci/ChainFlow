using ChainFlow.Models;

namespace ChainFlow.Interfaces.DataFlowsDependencies
{
    public interface IDataValidator<in TIn> where TIn : class
    {
        Task<OperationResult<bool>> ValidateAsync(TIn request, CancellationToken cancellationToken);
    }
}
