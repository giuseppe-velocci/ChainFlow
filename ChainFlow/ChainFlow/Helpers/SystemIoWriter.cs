using ChainFlow.Internals;

namespace ChainFlow.Helpers
{
    internal class SystemIoWriter : ISystemIoWriter
    {
        public Task WriteFile(string filepath, string content)
        {
            File.WriteAllText(filepath, content);
            return Task.CompletedTask;
        }
    }
}
