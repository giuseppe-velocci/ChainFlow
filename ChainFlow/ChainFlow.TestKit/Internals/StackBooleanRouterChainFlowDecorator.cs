using ChainFlow.ChainFlows;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;

namespace ChainFlow.TestKit
{
    class StackBooleanRouterChainFlowDecorator<TRouterLogic> : IBooleanRouterFlow<TRouterLogic> 
        where TRouterLogic : IRouterDispatcher<bool>
    {
        private readonly BooleanRouterFlow<TRouterLogic> _flow;
        private readonly IList<string> _stack;

        public StackBooleanRouterChainFlowDecorator(TRouterLogic routerLogic, IList<string> stack)
        {
            _flow = new BooleanRouterFlow<TRouterLogic>(routerLogic);
            _stack = stack;
        }

        public string Describe() => string.Empty;

        public Task<ProcessingResultWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            _stack.Add(typeof(BooleanRouterFlow<TRouterLogic>).GetFullName());
            return _flow.ProcessAsync(message, cancellationToken);
        }

        public void SetNext(IChainFlow next)
        {
            _flow.SetNext(next);
        }

        IBooleanRouterFlow<TRouterLogic> IBooleanRouterFlow<TRouterLogic>.WithRightFlow(IChainFlow flow)
        {
            _flow.WithRightFlow(flow);
            return this;
        }

        IBooleanRouterFlow<TRouterLogic> IBooleanRouterFlow<TRouterLogic>.WithLeftFlow(IChainFlow flow)
        {
            _flow.WithLeftFlow(flow);
            return this;
        }
    }
}