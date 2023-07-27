namespace ChainFlow.Interfaces
{
    public interface IChainBuilder
    {
        IChainBuilder With<T>() where T : IChainLink;
        IChainLink Build();
    }
}
