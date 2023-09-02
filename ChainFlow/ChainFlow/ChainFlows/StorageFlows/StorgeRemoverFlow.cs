using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.StorageFlows
{
    public class StorgeRemoverFlow<TIn> : AbstractChainFlow where TIn : class
    {
        protected readonly IStorageRemover<TIn> _remover;

        public StorgeRemoverFlow(IStorageRemover<TIn> remover)
        {
            _remover = remover;
        }

        public override string Describe() => $"Delete {typeof(TIn)} from storage";

        public async override Task<ProcessingResult> ProcessRequestAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            OperationResult<bool> deletionResult = await _remover.RemoveAsync((TIn)message.Request, cancellationToken);

            return deletionResult.Value ?
                ProcessingResult.CreateWithSuccess(message.Request) :
                ProcessingResult.CreateWithFailure(message.Request, deletionResult.Message);
        }
    }
}
