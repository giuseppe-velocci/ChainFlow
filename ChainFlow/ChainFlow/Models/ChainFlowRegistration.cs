using ChainFlow.Interfaces;

namespace ChainFlow.Models
{
    internal record class ChainFlowRegistration
    {
        public ChainFlowRegistration(Type type, Func<IChainFlow> chainLinkFactory)
        {
            LinkType = type.FullName!;
            ChainLinkFactory = chainLinkFactory;
        }

        public string LinkType { get; }
        public Func<IChainFlow> ChainLinkFactory { get; }
    }
}
