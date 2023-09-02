using ChainFlow.Documentables;
using ChainFlow.Helpers;
using ChainFlow.Interfaces.DataFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.DataFlows
{
    [DocumentFlowBehavior(Enums.DocumentFlowBehavior.TerminateOnFailure)]
    public class DataValidatorFlow<TIn> : AbstractChainFlow where TIn : class
    {
        protected readonly IDataValidator<TIn> _validator;

        public DataValidatorFlow(IDataValidator<TIn> validator)
        {
            _validator = validator;
        }

        public override string Describe() => $"Is {typeof(TIn).GetFullName()} valid?";

        public async override Task<ProcessingResult> ProcessRequestAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            OperationResult<bool> validationResult = await _validator.ValidateAsync((TIn)message.Request, cancellationToken);
            return validationResult.Value ?
                ProcessingResult.CreateWithSuccess(message.Request) :
                ProcessingResult.CreateWithFailure(message.Request, validationResult.Message);
        }
    }
}
