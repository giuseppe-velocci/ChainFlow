using ChainFlow.Helpers;
using ChainFlow.Interfaces.DataFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.DataFlows
{
    public class DataMapperFlow<TIn, TOut> : AbstractChainFlow where TIn : class where TOut : class
    {
        protected readonly IDataMapper<TIn, TOut> _mapper;

        public DataMapperFlow(IDataMapper<TIn, TOut> mapper)
        {
            _mapper = mapper;
        }

        public override string Describe() => $"Map {typeof(TIn).GetFullName()} to {typeof(TOut).GetFullName()}";

        public async override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            OperationResult<TOut> result = await _mapper.MapAsync((TIn)message.Request, cancellationToken);
            return result.ToProcessingRequestWithOutcome();
        }
    }
}
