using ChainFlow.ChainFlows;
using ChainFlow.Documentables;
using ChainFlow.Models;

namespace Console
{
    [DocumentFlowBehavior(ChainFlow.Enums.DocumentFlowBehavior.TerminateOnFailure)]
    internal class ValidateInputFlow : AbstractChainFlow
    {
        public override string Describe() => "Is user input valid?";

        public override Task<ProcessingResult> ProcessRequestAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            var input = (string)message.Request;

            var result = string.IsNullOrWhiteSpace(input) ?
                ProcessingResult.CreateWithFailure(input, "Invalid input") :
                ProcessingResult.CreateWithSuccess(input);

            return Task.FromResult(result);
        }
    }
}
