using ChainFlow.ChainFlows;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;

namespace ChainFlow.Documentables
{
    class TodoBooleanRouterChainFlow<T> : BooleanRouterFlow<T>, IChainFlow where T : IRouterDispatcher<bool>
    {
        private readonly string _description;
        public TodoBooleanRouterChainFlow(T routerLogic) : base(routerLogic)
        {
            _description = $"TODO RouterFlow {typeof(T).GetFullName()}";
        }

        public override string Describe() => _description;
    }
}
