using ChainFlow.Models;

namespace ChainFlow.Interfaces.DataFlowsDependencies
{
    public interface IDataMapper<TIn, TOut> where TIn : class where TOut : class
    {
        public Task<OperationResult<TOut>> MapAsync(TIn request, CancellationToken cancellationToken);
    }
}
