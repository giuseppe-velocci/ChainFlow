using ChainFlow.Interfaces.DataFlowsDependencies;
using ChainFlow.Models;

namespace Console.Validators
{
    internal class NameValidator : IDataValidator<string>
    {
        public Task<OperationResult<bool>> ValidateAsync(string request, CancellationToken cancellationToken)
        {
            OperationResult<bool> result = request.Length < 3 ?
                OperationResult<bool>.CreateWithFailure(false, "Invalid name: cannot be less than 3 chars") :
                OperationResult<bool>.CreateWithSuccess(true, "Valid name");

            return Task.FromResult(result);
        }
    }
}
