using ChainFlow.Documentables;
using ChainFlow.Models;
using ChainFlowUnitTest.Helper;
using FluentAssertions;

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
            };
            _sut = new (_registrations);
        }

        [Fact]
        public void ToString_WhenNoFlowsAreResolved_ReturnsEmptyString()
        {
            var _ = _sut.Build();
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
Success(Workflow is completed with success)

{_registrations.First().GetDocumentFlowId()} --> {_registrations.ElementAt(1).GetDocumentFlowId()}
{_registrations.ElementAt(1).GetDocumentFlowId()} --> Success
:::";
            var _ = _sut
                .With<FakeChainLink>()
                .With<FakeChainLink2>()
                .Build();
            _sut.ToString().Should().Be(expected);
        }
    }
}
