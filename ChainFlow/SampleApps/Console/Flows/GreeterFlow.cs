using ChainFlow.ChainFlows;
using ChainFlow.Models;

namespace Console.Flows
{
    internal class GreeterFlow : AbstractChainFlow
    {
        public override string Describe() => "Greet user by name";

        public override Task<ProcessingResult> ProcessRequestAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            return Task.FromResult(ProcessingResult.CreateWithSuccess(message.Request, $"Hello {message.Request}"));
        }
    }
}