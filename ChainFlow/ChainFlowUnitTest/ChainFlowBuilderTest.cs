using ChainFlow.ChainBuilder;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlowUnitTest.Helper;
using FluentAssertions;

namespace ChainFlowUnitTest
{
    public class ChainFlowBuilderTest
    {
        private readonly ChainFlowBuilder _sut;

        public ChainFlowBuilderTest()
        {
            _sut = new ChainFlowBuilder(new ChainFlowRegistration[] {
                new ChainFlowRegistration(typeof(FakeChainLink), () => new FakeChainLink()),
                new ChainFlowRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
            });
        }

        [Fact]
        public void Ctor_WhenNoLinkIsRegisterd_ThrowsException()
        {
            var act = () => new ChainFlowBuilder(Enumerable.Empty<ChainFlowRegistration>());
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WhenNullLinkIsRegistered_ThrowsException()
        {
            var act = () => new ChainFlowBuilder(null!);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void With_WhenNonRegisteredLinkIsPassed_ThrowsException()
        {
            var act = () => _sut.With<IChainFlow>();
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void With_WhenRegisteredLinkIsPassed_Succeed()
        {
            _sut.With<FakeChainLink>().Should().BeOfType<ChainFlowBuilder>();
        }

        [Fact]
        public void Build_WhenDeclarationIsValid_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink2>()
                .With<FakeChainLink>()
                .Build();
            chain.Should().BeOfType<FakeChainLink2>();
        }
    }
}