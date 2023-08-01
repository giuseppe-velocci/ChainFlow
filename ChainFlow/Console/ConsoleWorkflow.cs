using ChainFlow.Interfaces;
using ChainFlow.Workflows;

namespace Console
{
    internal class ConsoleWorkflow : AbstractWorkflowRunner
    {
        public ConsoleWorkflow(IChainFlowBuilder chainBuilder) : base(chainBuilder)
        {
            Workflow = chainBuilder
                .With<ValidateInputFlow>()
                .Build();
        }

        protected override IChainFlow Workflow { get; set; }

        public override string Describe() => "A sample console app with ChainFlow";

        public override string DescribeWorkflowEntryPoint() => "When user input is received";

        public override string GetWorkflowName() => nameof(ConsoleWorkflow);
    }
}
