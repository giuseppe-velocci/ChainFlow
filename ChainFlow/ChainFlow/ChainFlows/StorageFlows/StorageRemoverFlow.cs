using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.StorageFlows
{
    public class StorageRemoverFlow<TIn> : AbstractChainFlow where TIn : class
    {
        protected readonly IStorageRemover<TIn> _remover;

        public StorageRemoverFlow(IStorageRemover<TIn> remover)
        {
            _remover = remover;
        }

        public override string Describe() => $"Delete {typeof(TIn)} from storage";

        public async override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            OperationResult<bool> deletionResult = await _remover.RemoveAsync((TIn)message.Request, cancellationToken);
            return deletionResult.Value ?
                ProcessingResultWithOutcome.CreateWithSuccess(message.Request) :
                ProcessingResultWithOutcome.CreateWithFailure(message.Request, deletionResult.Message);
        }
    }
}
