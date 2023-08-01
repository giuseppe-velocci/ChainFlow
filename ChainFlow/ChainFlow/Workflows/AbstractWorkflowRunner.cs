using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.Workflows
{
    public abstract class AbstractWorkflowRunner : IWorkflow
    {
        private readonly IChainFlowBuilder _workflowBuilder = null!;
        protected abstract IChainFlow Workflow { get; set; }

        public AbstractWorkflowRunner(IChainFlowBuilder chainBuilder)
        {
            _workflowBuilder = chainBuilder;
        }

        public Task<ProcessingResultWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            return Workflow!.ProcessAsync(message, cancellationToken);
        }

        public string GetFlow()
        {
            if (Workflow is null)
            {
                _workflowBuilder.Build();
            }

            return _workflowBuilder.ToString()!;
        }

        public abstract string GetWorkflowName();
        public abstract string Describe();
        public abstract string DescribeWorkflowEntryPoint();
    }
}
