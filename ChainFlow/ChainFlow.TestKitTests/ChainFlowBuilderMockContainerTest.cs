using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.ChainFlows.StorageFlows;
using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlow.TestKit;
using ChainFlow.Workflows;
using FluentAssertions;

namespace ChainFlow.TestKitUnitTests
{
    public class ChainFlowBuilderMockContainerTest
    {
        private ChainFlowTestKitContainer _sut;

        public ChainFlowBuilderMockContainerTest()
        {
            _sut = new();
        }

        [Fact]
        public async Task Test1()
        {
            var builder = _sut.GetChainFlowBuilder<bool>((x) => new FakeWorkflow(x));
            FakeWorkflow workflow = new (builder);

            var result = await workflow.ProcessAsync(new ProcessingRequest(new Input { Id = 1, Value = "abc" }), CancellationToken.None);
            
            result.Outcome.Should().Be(FlowOutcome.Success);
        }
    }

    class FakeWorkflow : AbstractWorkflowRunner
    {
        protected override IChainFlow Workflow { get; set; }

        public FakeWorkflow(IChainFlowBuilder chainBuilder) : base(chainBuilder)
        {
            Workflow = chainBuilder
                .With<DataValidatorFlow<Input>>()
                .With<DataMapperFlow<Input, InputEnriched>>()
                .With<StorageReaderFlow<InputEnriched, InputEnriched>>()
                .WithBooleanRouter<InputFlowDispatcher>(
                    (x) => x
                        .With<DataMapperFlow<InputEnriched, Output>>()
                        .With<StorageWriterFlow<InputFlowDispatcher>>()
                        .Build(),
                    (x) => x
                        .With<StorageRemoverFlow<InputEnriched>>()
                        .Build()
                )
                .Build();
        }

        public override string Describe() => string.Empty;
        public override string DescribeWorkflowEntryPoint() => string.Empty;
        public override string GetWorkflowName() => string.Empty;
    }

    public class InputFlowDispatcher : IRouterDispatcher<bool>
    {
        private IFakeDependency _fakeDependency;

        public InputFlowDispatcher(IFakeDependency fakeDependency)
        {
            _fakeDependency = fakeDependency;
        }

        public string Describe() => string.Empty;

        public async Task<bool> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            var result = await _fakeDependency.ProcessAsync(message, cancellationToken);
            return result.Value;
        }
    }

    public interface IFakeDependency
    {
        Task<OperationResult<bool>> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken);
    }

    public class Input
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;
    }

    public class Output
    {
        public string Value { get; set; } = null!;
    }

    public class InputEnriched
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public IEnumerable<string> Properties { get; set; } = null!;
    }
}