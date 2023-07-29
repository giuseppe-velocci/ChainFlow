using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.Workflows
{
    public abstract class AbstractWorkflowRunner<T> : IWorkflow<T> where T : notnull
    {
        private readonly IChainFlowBuilder _chainBuilder = null!;
        private IChainFlow chain = null!;

        public AbstractWorkflowRunner(IChainFlowBuilder chainBuilder)
        {
            _chainBuilder = chainBuilder;
            chain = _chainBuilder.Build();
        }

        public async Task<T> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {

            var response = await chain.ProcessAsync(message, cancellationToken);
            return Outcome2T(response);
        }

        public abstract T Outcome2T(ProcessingRequestWithOutcome outcome);

        public string GetFlow() => _chainBuilder.ToString()!;

        public abstract string GetWorkflowName();
        public abstract string Describe();
        public abstract string DescribeWorkflowEntryPoint();
    }
}
