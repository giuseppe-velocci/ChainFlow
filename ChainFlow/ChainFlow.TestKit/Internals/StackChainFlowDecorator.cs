using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.TestKit
{
    class StackChainFlowDecorator : IChainFlow
    {
        private readonly IChainFlow _flow;
        private readonly IList<string> _stack;
        private readonly string _nameSuffix;

        public StackChainFlowDecorator(IChainFlow flow, IList<string> stack, string nameSuffix)
        {
            _flow = flow;
            _stack = stack;
            _nameSuffix = nameSuffix;
        }

        public string Describe() => string.Empty;

        public Task<ProcessingResult> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            _stack.Add(_flow.GetType().GetFullName(_nameSuffix));
            return _flow.ProcessAsync(message, cancellationToken);
        }

        public void SetNext(IChainFlow next)
        {
            _flow.SetNext(next);
        }
    }
}