using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows
{
    internal class BooleanRouterFlow<TRouterLogic> : AbstractChainFlow where TRouterLogic : IRouterDispatcher<bool> 
    {
        private IChainFlow _rightFlow = null!;
        private IChainFlow _leftFlow = null!;
        private readonly IRouterDispatcher<bool> _routerLogic;

        public BooleanRouterFlow(TRouterLogic routerLogic)
        {
            _routerLogic = routerLogic;
        }

        public BooleanRouterFlow<TRouterLogic> WithRightFlow(IChainFlow flow)
        {
            _rightFlow = flow;
            return this;
        }

        public BooleanRouterFlow<TRouterLogic> WithLeftFlow(IChainFlow flow)
        {
            _leftFlow = flow;
            return this;
        }

        public override async Task<ProcessingRequestWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            bool routerLogicOutcome = await _routerLogic.ProcessAsync(message, cancellationToken);
            return routerLogicOutcome ?
                await _rightFlow.ProcessAsync(message, cancellationToken) : 
                await _leftFlow.ProcessAsync(message, cancellationToken);
        }

        public override string Describe() =>_routerLogic.Describe();
    }
}
