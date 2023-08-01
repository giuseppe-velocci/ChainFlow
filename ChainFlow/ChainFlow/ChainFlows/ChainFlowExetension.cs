using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows
{
    internal static class ChainFlowExetension
    {
        public static Task<ProcessingResultWithOutcome> ProcessAsync(this IChainFlow flow, ProcessingResultWithOutcome message, CancellationToken cancellationToken)
        {
            return flow.ProcessAsync(new ProcessingRequest(message.Result), cancellationToken);
        }
    }
}
