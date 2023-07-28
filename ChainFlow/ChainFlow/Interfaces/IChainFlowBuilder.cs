namespace ChainFlow.Interfaces
{
    public interface IChainFlowBuilder
    {
        IChainFlowBuilder With<T>() where T : IChainFlow;
        IChainFlowBuilder WithBooleanRouter<TRouter>(
            Func<IChainFlowBuilder, IChainFlow> rightFlowFactory,
            Func<IChainFlowBuilder, IChainFlow> leftFlowFactory) where TRouter : IRouterLogic<bool>;
        IChainFlow Build();
    }
}
