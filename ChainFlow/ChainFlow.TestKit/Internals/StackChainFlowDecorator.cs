using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.TestKit
{
    class StackChainFlowDecorator : IChainFlow
    {
        private readonly IChainFlow _flow;
        private readonly IList<string> _stack;

        public StackChainFlowDecorator(IChainFlow flow, IList<string> stack)
        {
            _flow = flow;
            _stack = stack;
        }

        public string Describe() => string.Empty;

        public Task<ProcessingResultWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            _stack.Add(_flow.GetType().GetFullName());
            return _flow.ProcessAsync(message, cancellationToken);
        }

        public void SetNext(IChainFlow next)
        {
            _flow.SetNext(next);
        }
    }
}