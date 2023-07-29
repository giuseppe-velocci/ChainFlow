namespace ChainFlow.Interfaces
{
    public interface ISystemIoWriter
    {
        public Task WriteFile(string filepath, string content);
    }
}
