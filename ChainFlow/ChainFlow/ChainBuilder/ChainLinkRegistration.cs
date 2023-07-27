using ChainFlow.Interfaces;

namespace ChainFlow.ChainBuilder
{
    internal record class ChainLinkRegistration
    {
        public ChainLinkRegistration(Type type, Func<IChainLink> chainLinkFactory)
        {
            LinkType = type.FullName!;
            ChainLinkFactory = chainLinkFactory;
        }

        public string LinkType { get; }
        public Func<IChainLink> ChainLinkFactory { get; }


    }
}
