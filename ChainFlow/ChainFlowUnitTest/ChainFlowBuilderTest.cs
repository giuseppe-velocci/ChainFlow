using ChainFlow.ChainBuilder;
using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlowUnitTest.Helpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest
{
    public class ChainFlowBuilderTest
    {
        private readonly ChainFlowBuilder _sut;

        public ChainFlowBuilderTest()
        {
            _sut = new ChainFlowBuilder(new ChainFlowRegistration[] {
                new ChainFlowRegistration(typeof(FakeChainLink0), () => new FakeChainLink0()),
                new ChainFlowRegistration(typeof(FakeChainLink1), () => new FakeChainLink1()),
                new ChainFlowRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
                new ChainFlowRegistration(typeof(FakeChainLink3), () => new FakeChainLink3()),
                new ChainFlowRegistration(typeof(FakeChainLink4), () => new FakeChainLink4()),
                new ChainFlowRegistration(typeof(BooleanRouterFlow<IRouterLogic<bool>>), () => new BooleanRouterFlow<IRouterLogic<bool>>(new Mock<IRouterLogic<bool>>().Object)),
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
        public void With_WhenUnregisteredLinkIsPassed_ThrowsException()
        {
            var act = () => _sut.With<IChainFlow>();
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void With_WhenRegisteredLinkIsPassed_Succeed()
        {
            _sut.With<FakeChainLink0>().Should().BeOfType<ChainFlowBuilder>();
        }

        [Fact]
        public void WithBooleanRouter_WhenRegisteredLinksArePassed_Succeed()
        {
            _sut.WithBooleanRouter<IRouterLogic<bool>>(
                (x) => x.With<FakeChainLink2>().Build(),
                (x) => x.With<FakeChainLink3>().Build()
            ).Should().BeOfType<ChainFlowBuilder>();
        }

        [Fact]
        public void WithBooleanRouter_WhenUnregisteredLinkIsPassedAsRight_ThrowsException()
        {
            var act = () => _sut.WithBooleanRouter<IRouterLogic<bool>>(
                (x) => x.With<IChainFlow>().Build(),
                (x) => x.With<FakeChainLink3>().Build()
            );
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void WithBooleanRouter_WhenUnregisteredLinkIsPassedAsLeft_ThrowsException()
        {
            var act = () => _sut.WithBooleanRouter<IRouterLogic<bool>>(
                (x) => x.With<FakeChainLink3>().Build(),
                (x) => x.With<IChainFlow>().Build()
            );
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Build_WhenDeclarationIsValid_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .Build();
            chain.Should().BeOfType<FakeChainLink1>();
        }

        [Fact]
        public void Build_WhenDeclarationIsValidAlsoWithBooleanRouter_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .WithBooleanRouter<IRouterLogic<bool>>(
                    (x) => x.With<FakeChainLink2>().Build(),
                    (x) => x.With<FakeChainLink3>().Build()
                )
                .Build();
            chain.Should().BeOfType<FakeChainLink1>();
        }

        [Fact]
        public void Build_WhenDeclarationIsValidAlsoWithNestedBooleanRouter_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .WithBooleanRouter<IRouterLogic<bool>>(
                    (x) => x
                        .With<FakeChainLink2>()
                        .WithBooleanRouter<IRouterLogic<bool>>(
                            y => y.With<FakeChainLink4>().Build(),
                            y => y.With<FakeChainLink3>().Build())
                        .Build(),
                    (x) => x.With<FakeChainLink3>().Build()
                )
                .Build();
            chain.Should().BeOfType<FakeChainLink1>();
        }
    }
}