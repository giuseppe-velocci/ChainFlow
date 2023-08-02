using ChainFlow.Interfaces;
using ChainFlow.Internals;

namespace ChainFlow.Helpers
{
    public class ChainFlowNameResolver
    {
        public static string GetBooleanRouterChainFlowName<TRouterDispatcher>() where TRouterDispatcher : IRouterDispatcher<bool>
            => typeof(IBooleanRouterFlow<TRouterDispatcher>).GetFullName();
    }
}
