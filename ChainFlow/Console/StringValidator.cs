using ChainFlow.Interfaces.DataFlowsDependencies;
using ChainFlow.Models;

namespace Console
{
    internal class StringValidator : IDataValidator<string>
    {
        public Task<OperationResult<bool>> ValidateAsync(string request, CancellationToken cancellationToken)
        {
            OperationResult<bool> result = string.IsNullOrWhiteSpace(request) ?
                OperationResult<bool>.CreateWithFailure(false, "Invalid input: cannot be empty") :
                OperationResult<bool>.CreateWithSuccess(true, "Valid input");

            return Task.FromResult(result);
        }
    }
}
