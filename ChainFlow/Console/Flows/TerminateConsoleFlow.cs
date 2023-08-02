using ChainFlow.ChainFlows;
using ChainFlow.Models;

namespace Console.Flows
{
    internal class TerminateConsoleFlow : AbstractChainFlow
    {
        public override string Describe() => "Exit program";

        public override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            throw new OperationCanceledException();
        }
    }
}