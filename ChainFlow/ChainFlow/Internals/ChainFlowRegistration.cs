using ChainFlow.Helpers;
using ChainFlow.Interfaces;

namespace ChainFlow.Internals
{
    internal record class ChainFlowRegistration
    {
        public ChainFlowRegistration(Type type, Func<IChainFlow> chainLinkFactory)
        {
            ChainFlowName = type.GetFullName();
            ChainLinkFactory = chainLinkFactory;
        }

        public ChainFlowRegistration(Type type, Func<IChainFlow> chainLinkFactory, string nameSuffix)
        {
            ChainFlowName = type.GetFullName(nameSuffix);
            ChainLinkFactory = chainLinkFactory;
        }

        internal ChainFlowRegistration(string chainFlowName, Func<IChainFlow> chainLinkFactory)
        {
            ChainFlowName = chainFlowName;
            ChainLinkFactory = chainLinkFactory;
        }

        public string ChainFlowName { get; }
        public Func<IChainFlow> ChainLinkFactory { get; }
    }
}
