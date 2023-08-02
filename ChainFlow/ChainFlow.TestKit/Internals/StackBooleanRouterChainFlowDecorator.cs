using ChainFlow.ChainFlows;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;

namespace ChainFlow.TestKit
{
    class StackBooleanRouterChainFlowDecorator<TRouterDispatcher> : IBooleanRouterFlow<TRouterDispatcher> 
        where TRouterDispatcher : IRouterDispatcher<bool>
    {
        private readonly BooleanRouterFlow<TRouterDispatcher> _flow;
        private readonly IList<string> _stack;

        public StackBooleanRouterChainFlowDecorator(TRouterDispatcher routerLogic, IList<string> stack)
        {
            _flow = new BooleanRouterFlow<TRouterDispatcher>(routerLogic);
            _stack = stack;
        }

        public string Describe() => string.Empty;

        public Task<ProcessingResultWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            _stack.Add(ChainFlowNameResolver.GetBooleanRouterChainFlowName<TRouterDispatcher>());
            return _flow.ProcessAsync(message, cancellationToken);
        }

        public void SetNext(IChainFlow next)
        {
            _flow.SetNext(next);
        }

        IBooleanRouterFlow<TRouterDispatcher> IBooleanRouterFlow<TRouterDispatcher>.WithRightFlow(IChainFlow flow)
        {
            _flow.WithRightFlow(flow);
            return this;
        }

        IBooleanRouterFlow<TRouterDispatcher> IBooleanRouterFlow<TRouterDispatcher>.WithLeftFlow(IChainFlow flow)
        {
            _flow.WithLeftFlow(flow);
            return this;
        }
    }
}