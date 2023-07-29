using ChainFlow.ChainFlows;
using ChainFlow.Models;

namespace Console
{
    internal class ValidateInputFlow : AbstractChainFlow
    {
        public override string Describe() => "Validate user input";

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
