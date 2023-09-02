using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows
{
    public abstract class AbstractChainFlow : IChainFlow
    {
        private IChainFlow _next = null!;

        public abstract Task<ProcessingResult> ProcessRequestAsync(RequestToProcess message, CancellationToken cancellationToken);

        public abstract string Describe();

        public async Task<ProcessingResult> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            var result = await ProcessRequestAsync(message, cancellationToken);
            if (result.Outcome is FlowOutcome.Success)
            {
                return _next is null ?
                    result :
                    await _next.ProcessAsync(new RequestToProcess(result.Result), cancellationToken);
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
