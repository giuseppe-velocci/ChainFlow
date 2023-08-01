using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;

namespace ChainFlow.Internals
{
    internal interface IBooleanRouterFlow<TRouterLogic> : IChainFlow where TRouterLogic : IRouterDispatcher<bool>
    {
        IBooleanRouterFlow<TRouterLogic> WithRightFlow(IChainFlow flow);
        IBooleanRouterFlow<TRouterLogic> WithLeftFlow(IChainFlow flow);
    }
}