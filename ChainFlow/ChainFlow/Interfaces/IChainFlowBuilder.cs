using ChainFlow.Enums;

namespace ChainFlow.Interfaces
{
    public interface IChainFlowBuilder
    {
        IChainFlowBuilder With<T>() where T : IChainFlow;
        IChainFlow Build();
    }
}
