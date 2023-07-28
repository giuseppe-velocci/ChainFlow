using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows
{
    internal sealed class BooleanRouterFlow<TRouterLogix> : AbstractChainFlow where TRouterLogix : IRouterLogic<bool> 
    {
        private IChainFlow _rightFlow = null!;
        private IChainFlow _leftFlow = null!;
        private readonly IRouterLogic<bool> _routerLogic;

        public BooleanRouterFlow(TRouterLogix routerLogic)
        {
            _routerLogic = routerLogic;
        }

        public BooleanRouterFlow<TRouterLogix> WithRightFlow(IChainFlow flow)
        {
            _rightFlow = flow;
            return this;
        }

        public BooleanRouterFlow<TRouterLogix> WithLeftFlow(IChainFlow flow)
        {
            _leftFlow = flow;
            return this;
        }

        public override async Task<ProcessingRequestWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            bool routerLogicOutcome = await _routerLogic.ExcecuteAsync(message, cancellationToken);
            return routerLogicOutcome ?
                await _rightFlow.ProcessAsync(message, cancellationToken) : 
                await _leftFlow.ProcessAsync(message, cancellationToken);
        }

        public override string Describe() =>_routerLogic.Describe();
    }
}
