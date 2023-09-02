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
    public class ContainerTest
    {
        private readonly Container _sut;

        public ContainerTest()
        {
            _sut = new();
        }

        [Fact]
        public async Task ProcessAsync_WhenHappyPath_ReturnsSuccess()
        {
            IChainFlowBuilder builder = _sut.GetChainFlowBuilder<bool>((x) => new FakeWorkflow(x));
            FakeWorkflow workflow = new(builder);

            var result = await workflow.ProcessAsync(new RequestToProcess(new Input { Id = 1, Value = "abc" }), CancellationToken.None);

            result.Outcome.Should().Be(FlowOutcome.Success);
            _sut.VerifyWorkflowCallStack(new ChainFlowStack[]
            {
                new (typeof(DataValidatorFlow<Input>)),
                new (typeof(DataMapperFlow<Input, InputEnriched>)),
                new (typeof(StorageReaderFlow<InputEnriched, InputEnriched>)),
                new (typeof(InputFlowDispatcher)),
                new (typeof(DataMapperFlow<InputEnriched, Output>)),
                new (typeof(StorageWriterFlow<Output>)),
            });
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
                        .With<StorageWriterFlow<Output>>()
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
        private readonly IFakeDependency _fakeDependency;

        public InputFlowDispatcher(IFakeDependency fakeDependency)
        {
            _fakeDependency = fakeDependency;
        }

        public string Describe() => string.Empty;

        public async Task<bool> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            var result = await _fakeDependency.ProcessAsync(message, cancellationToken);
            return result.Value;
        }
    }

    public interface IFakeDependency
    {
        Task<OperationResult<bool>> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken);
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