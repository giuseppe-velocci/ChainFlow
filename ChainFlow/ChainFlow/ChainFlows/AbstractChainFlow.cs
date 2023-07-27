using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows
{
    public abstract class AbstractChainFlow : IChainFlow
    {
        private IChainFlow _next = null!;

        public abstract Task<ProcessingRequestWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken);

        public virtual string Describe()
        {
            return null!;
        }

        public async Task<ProcessingRequestWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            var result = await ProcessRequestAsync(message, cancellationToken);
            if (result.Outcome is FlowOutcome.Success)
            {
                return _next is null ?
                    result :
                    await _next.ProcessAsync(result, cancellationToken);
            }
            else
            {
                return result;
            }
        }

        public void SetNext(IChainFlow next)
        {
            _next = next;
        }
    }
}
