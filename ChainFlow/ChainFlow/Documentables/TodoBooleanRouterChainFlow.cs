using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;

namespace ChainFlow.Documentables
{
    class TodoBooleanRouterChainFlow : BooleanRouterFlow, IChainFlow
    {
        private readonly string _description;
        public TodoBooleanRouterChainFlow(IRouterDispatcher<bool> routerLogic, string dispatcherName) : base(routerLogic)
        {
            _description = $"TODO RouterFlow {dispatcherName}";
        }

        public override string Describe() => _description;
    }
}
