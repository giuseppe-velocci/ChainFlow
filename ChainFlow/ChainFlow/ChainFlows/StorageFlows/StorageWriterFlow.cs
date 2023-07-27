using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.StorageFlows
{
    internal class StorageWriterFlow<TIn> : AbstractChainFlow where TIn : class
    {
        protected readonly IStorageWriter<TIn> _writer;

        public StorageWriterFlow(IStorageWriter<TIn> writer)
        {
            _writer = writer;
        }

        public override string Describe() => $"Delete {typeof(TIn)} from storage";

        public async override Task<ProcessingRequestWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            OperationResult<bool> deletionResult = await _writer.WriteAsync((TIn)message.Request, cancellationToken);
            return deletionResult.Value ?
                ProcessingRequestWithOutcome.CreateWithSuccess(message.Request) :
                ProcessingRequestWithOutcome.CreateWithFailure(message.Request, deletionResult.Message);
        }
    }
}
