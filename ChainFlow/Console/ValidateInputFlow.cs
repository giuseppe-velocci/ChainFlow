using ChainFlow.ChainFlows;
using ChainFlow.Documentables;
using ChainFlow.Models;

namespace Console
{
    [DocumentFlowBehavior(ChainFlow.Enums.DocumentFlowBehavior.TerminateOnFailure)]
    internal class ValidateInputFlow : AbstractChainFlow
    {
        public override string Describe() => "Is user input valid?";

        public override Task<ProcessingResultWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            var input = (string)message.Request;

            var result = string.IsNullOrWhiteSpace(input) ?
                ProcessingResultWithOutcome.CreateWithFailure(input, "Invalid input") :
                ProcessingResultWithOutcome.CreateWithSuccess(input);

            return Task.FromResult(result);
        }
    }
}
