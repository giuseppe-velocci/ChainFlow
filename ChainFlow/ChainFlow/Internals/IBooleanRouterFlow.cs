using ChainFlow.Interfaces;

namespace ChainFlow.Internals
{
    internal interface IBooleanRouterFlow : IChainFlow
    {
        IBooleanRouterFlow WithRightFlow(IChainFlow flow);
        IBooleanRouterFlow WithLeftFlow(IChainFlow flow);
    }
}