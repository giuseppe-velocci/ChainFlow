using ChainFlow.ChainFlows;
using ChainFlow.Documentables;
using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlowUnitTest.Helper;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest
{
    public class DocumentChainFlowBuilderTest
    {
        private readonly DocumentChainFlowBuilder _sut;
        private readonly IEnumerable<ChainFlowRegistration> _registrations;

        public DocumentChainFlowBuilderTest()
        {
            _registrations = new ChainFlowRegistration[] {
                new ChainFlowRegistration(typeof(FakeChainLink), () => new FakeChainLink()),
                new ChainFlowRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
                new ChainFlowRegistration(typeof(FakeChainLink3), () => new FakeChainLink3()),
                new ChainFlowRegistration(typeof(FakeChainLink4), () => new FakeChainLink4()),
                new ChainFlowRegistration(typeof(FakeChainLink5), () => new FakeChainLink5()),
                new ChainFlowRegistration(typeof(BooleanRouterFlow<IRouterLogic<bool>>), () => new BooleanRouterFlow<IRouterLogic<bool>>(new Mock<IRouterLogic<bool>>().Object)),
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
        public void ToString_WhenSingleFlowIsResolved_ReturnsFlowString()
        {
            string expected =
$@"::: mermaid
graph TD;
{_registrations.First().GetDocumentFlowId()}({_registrations.First().ChainLinkFactory().Describe()})
Success(Workflow is completed with success)

{_registrations.First().GetDocumentFlowId()} --> Success
:::";
            var _ = _sut
                .With<FakeChainLink>()
                .Build();
            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenMultipleFlowsAreResolved_ReturnsFlowString()
        {
            string expected =
$@"::: mermaid
graph TD;
{_registrations.First().GetDocumentFlowId()}({_registrations.First().ChainLinkFactory().Describe()})
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
{_registrations.ElementAt(5).GetDocumentFlowId()}{{{_registrations.ElementAt(5).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(2).GetDocumentFlowId()}({_registrations.ElementAt(2).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)
{_registrations.ElementAt(3).GetDocumentFlowId()}({_registrations.ElementAt(3).ChainLinkFactory().Describe()})
{_registrations.ElementAt(4).GetDocumentFlowId()}({_registrations.ElementAt(4).ChainLinkFactory().Describe()})

{_registrations.First().GetDocumentFlowId()} --> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --True--> {_registrations.ElementAt(2).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --False--> {_registrations.ElementAt(3).GetDocumentFlowId()}
{_registrations.ElementAt(2).GetDocumentFlowId()} --> {_registrations.ElementAt(4).GetDocumentFlowId()}
{_registrations.ElementAt(3).GetDocumentFlowId()} --> {_registrations.ElementAt(4).GetDocumentFlowId()}
{_registrations.ElementAt(4).GetDocumentFlowId()} --> Success
:::";
            var _ = _sut
                .With<FakeChainLink>()
                .With<FakeChainLink2>()
                .WithBooleanRouter<IRouterLogic<bool>>(
                    x => x.With<FakeChainLink3>().Build(),
                    x => x.With<FakeChainLink4>().Build()
                )
                .With<FakeChainLink5>()
                .Build();
            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenMultipleFlowsWithABooleanRouterAreResolved_ReturnsFlowString()
        {
            string expected =
    $@"::: mermaid
graph TD;
{_registrations.First().GetDocumentFlowId()}({_registrations.First().ChainLinkFactory().Describe()})
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)

{_registrations.First().GetDocumentFlowId()} --> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(1).GetDocumentFlowId()} --> Success
:::";
            var _ = _sut
                .With<FakeChainLink>()
                .With <FakeChainLink2>()
                .Build();
            _sut.ToString().Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenFlowsWithBooleanRouterAreResolvedWithDifferentOutcomes_ReturnsFlowString()
        {
            string expected =
$@"::: mermaid
graph TD;
{_registrations.First().GetDocumentFlowId()}({_registrations.First().ChainLinkFactory().Describe()})
{_registrations.ElementAt(1).GetDocumentFlowId()}({_registrations.ElementAt(1).ChainLinkFactory().Describe()})
{_registrations.ElementAt(5).GetDocumentFlowId()}{{{_registrations.ElementAt(5).ChainLinkFactory().Describe()}}}
{_registrations.ElementAt(2).GetDocumentFlowId()}({_registrations.ElementAt(2).ChainLinkFactory().Describe()})
Success(Workflow is completed with success)
{_registrations.ElementAt(3).GetDocumentFlowId()}({_registrations.ElementAt(3).ChainLinkFactory().Describe()})
Failure(Workflow is completed with failure)

{_registrations.First().GetDocumentFlowId()} --> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --True--> {_registrations.ElementAt(2).GetDocumentFlowId()}
{_registrations.ElementAt(5).GetDocumentFlowId()} --False--> {_registrations.ElementAt(3).GetDocumentFlowId()}
{_registrations.ElementAt(3).GetDocumentFlowId()} --> Failure
{_registrations.ElementAt(2).GetDocumentFlowId()} --> Success
:::";
            var _ = _sut
                .With<FakeChainLink>()
                .With<FakeChainLink2>()
                .WithBooleanRouter<IRouterLogic<bool>>(
                    x => x.With<FakeChainLink3>().Build(FlowOutcome.Success),
                    x => x.With<FakeChainLink4>().Build(FlowOutcome.Failure)
                )
                .Build();
            _sut.ToString().Should().Be(expected);
        }
    }
}
