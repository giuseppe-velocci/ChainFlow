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

        public async override Task<ProcessingResult> ProcessRequestAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            OperationResult<bool> deletionResult = await _writer.WriteAsync((TIn)message.Request, cancellationToken);
            return deletionResult.Value ?
                ProcessingResult.CreateWithSuccess(message.Request) :
                ProcessingResult.CreateWithFailure(message.Request, deletionResult.Message);
        }
    }
}
