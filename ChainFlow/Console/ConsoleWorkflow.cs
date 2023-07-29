using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlow.Workflows;

namespace Console
{
    internal class ConsoleWorkflow : AbstractWorkflowRunner<string>
    {
        public ConsoleWorkflow(IChainFlowBuilder chainBuilder) : base(chainBuilder)
        { }

        public override string Describe() => "A sample console app with ChainFlow";

        public override string DescribeWorkflowEntryPoint() => "When user input is received";

        public override string GetWorkflowName() => nameof(ConsoleWorkflow);

        public override string Outcome2T(ProcessingRequestWithOutcome outcome)
        {
            throw new NotImplementedException();
        }
    }
}
