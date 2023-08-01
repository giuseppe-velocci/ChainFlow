using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows
{
    internal class BooleanRouterFlow<TRouterDispatcher> : AbstractChainFlow, IBooleanRouterFlow<TRouterDispatcher> 
        where TRouterDispatcher : IRouterDispatcher<bool>
    {
        private IChainFlow _rightFlow = null!;
        private IChainFlow _leftFlow = null!;
        private readonly IRouterDispatcher<bool> _routerLogic;

        public BooleanRouterFlow(TRouterDispatcher routerLogic)
        {
            _routerLogic = routerLogic;
        }

        public IBooleanRouterFlow<TRouterDispatcher> WithRightFlow(IChainFlow flow)
        {
            _rightFlow = flow;
            return this;
        }

        public IBooleanRouterFlow<TRouterDispatcher> WithLeftFlow(IChainFlow flow)
        {
            _leftFlow = flow;
            return this;
        }

        public override async Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            bool routerLogicOutcome = await _routerLogic.ProcessAsync(message, cancellationToken);
            return routerLogicOutcome ?
                await _rightFlow.ProcessAsync(message, cancellationToken) :
                await _leftFlow.ProcessAsync(message, cancellationToken);
        }

        public override string Describe() => _routerLogic.Describe();
    }
}
