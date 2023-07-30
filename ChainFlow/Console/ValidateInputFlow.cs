using ChainFlow.ChainFlows;
using ChainFlow.Documentables;
using ChainFlow.Models;

namespace Console
{
    [DocumentFlowBehavior(ChainFlow.Enums.DocumentFlowBehavior.TerminateOnFailure)]
    internal class ValidateInputFlow : AbstractChainFlow
    {
        public override string Describe() => "Is user input valid?";

        public override Task<ProcessingRequestWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            var input = (string)message.Request;

            var result = string.IsNullOrWhiteSpace(input) ?
                ProcessingRequestWithOutcome.CreateWithFailure(input, "Invalid input") :
                ProcessingRequestWithOutcome.CreateWithSuccess(input);

            return Task.FromResult(result);
        }
    }
}
