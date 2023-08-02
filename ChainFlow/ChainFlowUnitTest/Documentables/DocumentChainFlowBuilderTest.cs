using ChainFlow.ChainFlows;
using ChainFlow.Documentables;
using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;
using ChainFlowUnitTest.TestHelpers;
using FluentAssertions;

namespace ChainFlowUnitTest.Documentables
{
    public class DocumentChainFlowBuilderTest
    {
        private readonly DocumentChainFlowBuilder _sut;
        private readonly IEnumerable<ChainFlowRegistration> _registrations;

        public DocumentChainFlowBuilderTest()
        {
            _registrations = new ChainFlowRegistration[] {
                new ChainFlowRegistration(typeof(FakeChainLink0), () => new FakeChainLink0()),
                new ChainFlowRegistration(typeof(FakeChainLink1), () => new FakeChainLink1()),
                new ChainFlowRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
                new ChainFlowRegistration(typeof(FakeChainLink3), () => new FakeChainLink3()),
                new ChainFlowRegistration(typeof(FakeChainLink4), () => new FakeChainLink4()),
                new ChainFlowRegistration(typeof(IBooleanRouterFlow<RouterLogic>), () => new BooleanRouterFlow<RouterLogic>(new RouterLogic())),
                new ChainFlowRegistration(typeof(IBooleanRouterFlow<RouterLogic1>), () => new BooleanRouterFlow<RouterLogic1>(new RouterLogic1())),
                new ChainFlowRegistration(typeof(ExitOnFailFlow), () => new ExitOnFailFlow()),
            };
            _sut = new(_registrations);
        }

        [Fact]
        public void ToString_WhenNoFlowsAreResolved_ReturnsEmptyString()
        {
            var _ = _sut.Build(FlowOutcome.Success);
            _sut.ToString().Should().BeEmpty();
        }

        [Fact]
        public void ToString_WhenUnregisteredFlowIsPassed_RetrunsFlowString()
        {
            var expected =
$@"_\W?\d+\(TODO IChainFlow\)
Success\(Workflow is completed with success\)

_\W?\d+ --> Success";
            var _ = _sut
                .With<IChainFlow>()
                .Build();

            _sut.ToString().Should().MatchRegex(expected);
        }

        [Fact]
        public void ToString_WhenUnregisteredBooleanRouterFlowIsPassed_RetrunsFlowString()
        {
            var expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}\({_registrations.ElementAt(0).ChainLinkFactory().Describe()}\)
_\W?\d+\{{TODO RouterFlow IRouterDispatcher\<Boolean\>\}}
{_registrations.ElementAt(1).GetDocumentFlowId()}\({_registrations.ElementAt(1).ChainLinkFactory().Describe()}\)
_\W?\d+\(TODO IChainFlow\)
Success\(Workflow is completed with success\)
Failure\(Workflow is completed with failure\)

{_registrations.ElementAt(0).GetDocumentFlowId()} --> _\W?\d+\
_\W?\d+\ --True--> {_registrations.ElementAt(1).GetDocumentFlowId()}
_\W?\d+\ --False--> _\W?\d+\
_\W?\d+ --> Failure
{_registrations.ElementAt(1).GetDocumentFlowId()} --> Success";
            var _ = _sut
                .With<FakeChainLink0>()
                .WithBooleanRouter<IRouterDispatcher<bool>>(
                    (x) => x.With<FakeChainLink1>().Build(FlowOutcome.Success),
                    (x) => x.With<IChainFlow>().Build(FlowOutcome.Failure)
                )
                .Build();

            _sut.ToString().Should().MatchRegex(expected);
        }

        [Fact]
        public void ToString_WhenSingleFlowIsResolved_ReturnsFlowString()
        {
            string expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}({_registrations.ElementAt(0).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)

{_registrations.ElementAt(0).GetDocumentFlowId()} --> Success";
            var _ = _sut
                .With<FakeChainLink0>()
                .Build();

            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenMultipleFlowsAreResolved_ReturnsFlowString()
        {
            string expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}({_registrations.ElementAt(0).ChainLinkFactory().Describe()})
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
{_registrations.ElementAt(5).GetDocumentFlowId()}{{{_registrations.ElementAt(5).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(2).GetDocumentFlowId()}({_registrations.ElementAt(2).ChainLinkFactory().Describe()})
{_registrations.ElementAt(3).GetDocumentFlowId()}({_registrations.ElementAt(3).ChainLinkFactory().Describe()})
{_registrations.ElementAt(4).GetDocumentFlowId()}({_registrations.ElementAt(4).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)

{_registrations.ElementAt(0).GetDocumentFlowId()} --> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(1).GetDocumentFlowId()} --> {_registrations.ElementAt(5).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --True--> {_registrations.ElementAt(2).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --False--> {_registrations.ElementAt(3).GetDocumentFlowId()}
{_registrations.ElementAt(2).GetDocumentFlowId()} --> {_registrations.ElementAt(4).GetDocumentFlowId()}
{_registrations.ElementAt(3).GetDocumentFlowId()} --> {_registrations.ElementAt(4).GetDocumentFlowId()}
{_registrations.ElementAt(4).GetDocumentFlowId()} --> Success";
            var _ = _sut
                .With<FakeChainLink0>()
                .With<FakeChainLink1>()
                .WithBooleanRouter<RouterLogic>(
                    x => x.With<FakeChainLink2>().Build(),
                    x => x.With<FakeChainLink3>().Build()
                )
                .With<FakeChainLink4>()
                .Build();

            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenMultipleFlowsWithABooleanRouterAreResolved_ReturnsFlowString()
        {
            string expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}({_registrations.ElementAt(0).ChainLinkFactory().Describe()})
{_registrations.ElementAt(5).GetDocumentFlowId()}{{{_registrations.ElementAt(5).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
{_registrations.ElementAt(2).GetDocumentFlowId()}({_registrations.ElementAt(2).ChainLinkFactory().Describe()})
{_registrations.ElementAt(6).GetDocumentFlowId()}{{{_registrations.ElementAt(6).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(3).GetDocumentFlowId()}({_registrations.ElementAt(3).ChainLinkFactory().Describe()})
{_registrations.ElementAt(4).GetDocumentFlowId()}({_registrations.ElementAt(4).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)
TransientFailure(Workflow is completed with transient failure)

{_registrations.ElementAt(0).GetDocumentFlowId()} --> {_registrations.ElementAt(5).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --True--> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --False--> {_registrations.ElementAt(6).GetDocumentFlowId()}
{_registrations.ElementAt(1).GetDocumentFlowId()} --> {_registrations.ElementAt(2).GetDocumentFlowId()}
{_registrations.ElementAt(6).GetDocumentFlowId()} --True--> {_registrations.ElementAt(3).GetDocumentFlowId()}
{_registrations.ElementAt(6).GetDocumentFlowId()} --False--> {_registrations.ElementAt(4).GetDocumentFlowId()}
{_registrations.ElementAt(4).GetDocumentFlowId()} --> TransientFailure
{_registrations.ElementAt(2).GetDocumentFlowId()} --> Success
{_registrations.ElementAt(3).GetDocumentFlowId()} --> Success";
            var _ = _sut
                .With<FakeChainLink0>()
                .WithBooleanRouter<RouterLogic>(
                    (x) => x
                        .With<FakeChainLink1>()
                        .With<FakeChainLink2>()
                        .Build(FlowOutcome.Success),
                    (x) => x.WithBooleanRouter<RouterLogic1>(
                        (y) => y
                            .With<FakeChainLink3>()
                            .Build(FlowOutcome.Success),
                        (y) => y
                            .With<FakeChainLink4>()
                            .Build(FlowOutcome.TransientFailure)
                    )
                    .Build()
                )
                .Build();

            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenFlowsWithBooleanRouterAreResolvedWithDifferentOutcomes_ReturnsFlowString()
        {
            string expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}({_registrations.ElementAt(0).ChainLinkFactory().Describe()})
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
{_registrations.ElementAt(5).GetDocumentFlowId()}{{{_registrations.ElementAt(5).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(2).GetDocumentFlowId()}({_registrations.ElementAt(2).ChainLinkFactory().Describe()})
{_registrations.ElementAt(3).GetDocumentFlowId()}({_registrations.ElementAt(3).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)
Failure(Workflow is completed with failure)

{_registrations.ElementAt(0).GetDocumentFlowId()} --> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(1).GetDocumentFlowId()} --> {_registrations.ElementAt(5).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --True--> {_registrations.ElementAt(2).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --False--> {_registrations.ElementAt(3).GetDocumentFlowId()}
{_registrations.ElementAt(3).GetDocumentFlowId()} --> Failure
{_registrations.ElementAt(2).GetDocumentFlowId()} --> Success";
            var _ = _sut
                .With<FakeChainLink0>()
                .With<FakeChainLink1>()
                .WithBooleanRouter<RouterLogic>(
                    x => x.With<FakeChainLink2>().Build(FlowOutcome.Success),
                    x => x.With<FakeChainLink3>().Build(FlowOutcome.Failure)
                )
                .Build();

            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenFlowWithExitDocumentBehaviorIsResolved_ReturnsFlowString()
        {
            string expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}({_registrations.ElementAt(0).ChainLinkFactory().Describe()})
{_registrations.ElementAt(7).GetDocumentFlowId()}{{{_registrations.ElementAt(7).ChainLinkFactory().Describe()}}}
Failure(Workflow is completed with failure)
Success(Workflow is completed with success)

{_registrations.ElementAt(7).GetDocumentFlowId()} --False--> Failure
{_registrations.ElementAt(0).GetDocumentFlowId()} --> {_registrations.ElementAt(7).GetDocumentFlowId()}
{_registrations.ElementAt(7).GetDocumentFlowId()} --True--> Success";

            var _ = _sut
                .With<FakeChainLink0>()
                .With<ExitOnFailFlow>()
                .Build();

            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenFlowWithExitDocumentBehaviorIsResolvedHavingNextFlows_ReturnsFlowString()
        {
            string expected =
$@"{_registrations.ElementAt(0).GetDocumentFlowId()}({_registrations.ElementAt(0).ChainLinkFactory().Describe()})
{_registrations.ElementAt(7).GetDocumentFlowId()}{{{_registrations.ElementAt(7).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
Failure(Workflow is completed with failure)
Success(Workflow is completed with success)

{_registrations.ElementAt(7).GetDocumentFlowId()} --False--> Failure
{_registrations.ElementAt(0).GetDocumentFlowId()} --> {_registrations.ElementAt(7).GetDocumentFlowId()}
{_registrations.ElementAt(7).GetDocumentFlowId()} --True--> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(1).GetDocumentFlowId()} --> Success";

            var _ = _sut
                .With<FakeChainLink0>()
                .With<ExitOnFailFlow>()
                .With<FakeChainLink1>()
                .Build();

            _sut.ToString().Should().Be(expected);
        }
    }

    class RouterLogic : IRouterDispatcher<bool>
    {
        public virtual string Describe() => "Is main logic valid @5?";

        public Task<bool> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken) =>
            Task.FromResult(true);
    }

    class RouterLogic1 : RouterLogic
    {
        public override string Describe() => "Is secondary logic valid @6?";
    }

    [DocumentFlowBehavior(DocumentFlowBehavior.TerminateOnFailure)]
    class ExitOnFailFlow : IChainFlow
    {
        public string Describe() => "Is data correct @7?";

        public Task<ProcessingResultWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
            => Task.FromResult(new ProcessingResultWithOutcome(new object(), FlowOutcome.Success, string.Empty));

        public void SetNext(IChainFlow next)
        { }
    }
}
