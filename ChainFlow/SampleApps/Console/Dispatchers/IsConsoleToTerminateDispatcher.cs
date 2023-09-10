using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace Console.Dispatchers
{
    internal class IsConsoleToTerminateDispatcher : IRouterDispatcher<bool>
    {
        public string Describe() => "Has user terminated input sequence?";

        public Task<bool> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            return Task.FromResult(((string)message.Request).ToLower() == "exit");
        }
    }
}