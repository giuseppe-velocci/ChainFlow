using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ChainFlow.Interfaces
{
    internal interface ISystemIoWriter
    {
        public Task WriteFile(string filepath, string content);
    }
}
