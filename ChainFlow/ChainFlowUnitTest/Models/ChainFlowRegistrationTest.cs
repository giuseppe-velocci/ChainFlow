using ChainFlow.Helpers;
using ChainFlow.Internals;
using ChainFlowUnitTest.TestHelpers;
using FluentAssertions;

namespace ChainFlowUnitTest.Models
{
    public class ChainFlowRegistrationTest
    {
        [Fact]
        public void ChainLinkFactory_WhenDeclarationIsValid_ReturnsInstance()
        {
            ChainFlowRegistration sut = new(typeof(FakeChainLink2), () => new FakeChainLink2());
            sut.ChainLinkFactory().Should().BeOfType<FakeChainLink2>();
        }

        [Fact]
        public void LinkType_WhenDeclarationIsValid_ReturnsInstance()
        {
            ChainFlowRegistration sut = new(typeof(FakeChainLink2), () => new FakeChainLink2());
            sut.LinkType.Should().Be(typeof(FakeChainLink2).GetFullName());
        }
    }
}
