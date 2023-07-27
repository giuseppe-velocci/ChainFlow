using ChainFlow.Interfaces;

namespace ChainFlow.Documentables
{
    internal class DocumentChainFlowBuilder : IChainFlowBuilder
    {
        public IChainFlow Build()
        {
            throw new NotImplementedException();
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
