using ChainFlow.ChainBuilder;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using FluentAssertions;

namespace ChainFlowUnitTest
{
    public class ChainFlowBuilderTest
    {
        private readonly ChainBuilder Sut;

        public ChainFlowBuilderTest()
        {
            Sut = new ChainBuilder(new ChainLinkRegistration[] {
                new ChainLinkRegistration(typeof(FakeChainLink), () => new FakeChainLink()),
                new ChainLinkRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
            });
        }

        [Fact]
        public void Ctor_WhenNoLinkIsRegisterd_ThrowsException()
        {
            var act = () => new ChainBuilder(Enumerable.Empty<ChainLinkRegistration>());
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WhenNullLinkIsRegistered_ThrowsException()
        {
            var act = () => new ChainBuilder(null!);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void With_WhenNonRegisteredLinkIsPassed_ThrowsException()
        {
            var act = () => Sut.With<IChainFlow>();
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void With_WhenRegisteredLinkIsPassed_Succeed()
        {
            Sut.With<FakeChainLink>().Should().BeOfType<ChainBuilder>();
        }

        [Fact]
        public void Build_WhenDeclarationIsValid_ReturnsFirstLink()
        {
            var chain = Sut
                .With<FakeChainLink2>()
                .With<FakeChainLink>()
                .Build();
            chain.Should().BeOfType<FakeChainLink2>();
        }
    }

    class FakeChainLink : IChainFlow
    {
        public string Describe()
        {
            throw new NotImplementedException();
        }

        public Task<ProcessingRequestWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SetNext(IChainFlow next)
        {
            return;
        }
    }

    class FakeChainLink2 : FakeChainLink { }
}