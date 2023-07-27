using ChainFlow.Helpers;
using ChainFlow.Interfaces;

namespace ChainFlow.Internals
{
    internal record class ChainFlowRegistration
    {
        public ChainFlowRegistration(Type type, Func<IChainFlow> chainLinkFactory)
        {
            LinkType = type.GetFullName();
            ChainLinkFactory = chainLinkFactory;
        }

        public string LinkType { get; }
        public Func<IChainFlow> ChainLinkFactory { get; }
    }
}
