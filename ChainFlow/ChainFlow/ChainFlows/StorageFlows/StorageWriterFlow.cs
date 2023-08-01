using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.StorageFlows
{
    public class StorageWriterFlow<TIn> : AbstractChainFlow where TIn : class
    {
        protected readonly IStorageWriter<TIn> _writer;

        public StorageWriterFlow(IStorageWriter<TIn> writer)
        {
            _writer = writer;
        }

        public override string Describe() => $"Delete {typeof(TIn)} from storage";

        public async override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            OperationResult<bool> deletionResult = await _writer.WriteAsync((TIn)message.Request, cancellationToken);
            return deletionResult.Value ?
                ProcessingResultWithOutcome.CreateWithSuccess(message.Request) :
                ProcessingResultWithOutcome.CreateWithFailure(message.Request, deletionResult.Message);
        }
    }
}
