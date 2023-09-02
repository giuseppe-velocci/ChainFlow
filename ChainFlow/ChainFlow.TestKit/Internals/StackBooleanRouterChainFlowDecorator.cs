using ChainFlow.ChainFlows;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;

namespace ChainFlow.TestKit
{
    class StackBooleanRouterChainFlowDecorator : IBooleanRouterFlow
    {
        private readonly BooleanRouterFlow _flow;
        private readonly IList<string> _stack;
        private readonly string _stackName;

        public StackBooleanRouterChainFlowDecorator(IRouterDispatcher<bool> routerLogic, IList<string> stack)
        {
            _flow = new BooleanRouterFlow(routerLogic);
            _stack = stack;
            _stackName = routerLogic.GetType().GetFullName();
        }

        public string Describe() => string.Empty;

        public Task<ProcessingResult> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            _stack.Add(_stackName);
            return _flow.ProcessAsync(message, cancellationToken);
        }

        public void SetNext(IChainFlow next)
        {
            _flow.SetNext(next);
        }

        IBooleanRouterFlow IBooleanRouterFlow.WithRightFlow(IChainFlow flow)
        {
            _flow.WithRightFlow(flow);
            return this;
        }

        IBooleanRouterFlow IBooleanRouterFlow.WithLeftFlow(IChainFlow flow)
        {
            _flow.WithLeftFlow(flow);
            return this;
        }
    }
}