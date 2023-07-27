using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.ChainLinks
{
    public abstract class AbstractChainLink : IChainLink
    {
        private IChainLink _next = null!;

        public abstract Task<ProcessingRequest> ProcessRequestAsync(ProcessingRequest message);

        public virtual string Describe()
        {
            throw new NotImplementedException();
        }

        public async Task<ProcessingRequest> ProcessAsync(ProcessingRequest message)
        {
            var result = await ProcessRequestAsync(message);
            if (result.Result.FlowOutcome is FlowOutcome.Success)
            {
                return await _next.ProcessAsync(result);
            }
            else
            {
                return result;
            }
        }

        public void SetNext(IChainLink next)
        {
            _next = next;
        }
    }
}
