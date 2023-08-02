using ChainFlow.ChainFlows;
using ChainFlow.Models;

namespace Console.Flows
{
    internal class GreeterFlow : AbstractChainFlow
    {
        public override string Describe() => "Greet user by name";

        public override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            return Task.FromResult(ProcessingResultWithOutcome.CreateWithSuccess(message.Request, $"Hello {message.Request}"));
        }
    }
}