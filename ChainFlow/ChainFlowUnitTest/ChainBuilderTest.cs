using ChainFlow;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using FluentAssertions;
using System.ComponentModel;

namespace ChainFlowUnitTest
{
    public class ChainBuilderTest
    {
        private readonly ChainBuilder Sut;

        public ChainBuilderTest()
        {
            Sut = new ChainBuilder(new ChainLinkRegistration[] { 
                new ChainLinkRegistration(typeof(FakeChainLink), () => new FakeChainLink()),
                new ChainLinkRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
            });
        }

        [Fact]
        public void With_WhenNonRegisteredLinkIsPassed_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Sut.With<IChainLink>());
        }

        [Fact]
        public void With_WhenRegisteredLinkIsPassed_Succeed()
        {
            Sut.With<FakeChainLink>().Should().BeOfType<ChainBuilder>();
        }

        [Fact]
        public void Ctor_WhenNoLinkIsRegisterd_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new ChainBuilder(Enumerable.Empty<ChainLinkRegistration>()));
        }
        
        [Fact]
        public void Ctor_WhenNullLinkIsRegistered_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new ChainBuilder(null!));
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

    class FakeChainLink : IChainLink
    {
        public string Describe()
        {
            throw new NotImplementedException();
        }

        public Task<ProcessingMessage> ProcessAsync(ProcessingMessage message)
        {
            throw new NotImplementedException();
        }

        public void SetNext(IChainLink next)
        {
            return;
        }
    }

    class FakeChainLink2 : FakeChainLink { }
}