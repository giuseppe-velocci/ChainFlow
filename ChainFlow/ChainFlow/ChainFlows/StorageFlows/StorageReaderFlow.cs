using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.StorageFlows
{
    public class StorageReaderFlow<TIn, TOut> : AbstractChainFlow where TIn : class where TOut : class
    {
        protected readonly IStorageReader<TIn, TOut> _reader;

        public StorageReaderFlow(IStorageReader<TIn, TOut> reader)
        {
            _reader = reader;
        }

        public override string Describe() => $"Read {typeof(TOut)} from storage";

        public async override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            OperationResult<TOut> result = await _reader.ReadAsync((TIn)message.Request, cancellationToken);
            return result.ToProcessingRequestWithOutcome();
        }
    }
}
