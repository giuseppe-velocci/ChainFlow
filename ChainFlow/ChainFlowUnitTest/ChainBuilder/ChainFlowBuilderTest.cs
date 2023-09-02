using ChainFlow.ChainBuilder;
using ChainFlow.ChainFlows;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlowUnitTest.TestHelpers;
using FluentAssertions;
using Moq;

namespace ChainFlowUnitTest.ChainBuilder
{
    public class ChainFlowBuilderTest
    {
        private readonly ChainFlowBuilder _sut;
        private static readonly IChainFlow ChainFlow0 = new FakeChainLink0();
        private static readonly IChainFlow ChainFlow001 = new FakeChainLink0();
        private readonly ChainFlowRegistration[] _registrations = new ChainFlowRegistration[] {
            new ChainFlowRegistration(typeof(FakeChainLink0), () => ChainFlow0),
            new ChainFlowRegistration(typeof(FakeChainLink0), () => ChainFlow001, "01"),
            new ChainFlowRegistration(typeof(FakeChainLink1), () => new FakeChainLink1()),
            new ChainFlowRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
            new ChainFlowRegistration(typeof(FakeChainLink3), () => new FakeChainLink3()),
            new ChainFlowRegistration(typeof(FakeChainLink4), () => new FakeChainLink4()),
            new ChainFlowRegistration(typeof(IRouterDispatcher<bool>), () => new BooleanRouterFlow(new Mock<IRouterDispatcher<bool>>().Object)),
        };

        public ChainFlowBuilderTest()
        {
            _sut = new ChainFlowBuilder(_registrations);
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
        public void With_WhenSuffixAndUnregisteredLinkIsPassed_ThrowsException()
        {
            var act = () => _sut.With<IChainFlow>("01");
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void With_WhenSuffixAndRegisteredLinkIsPassed_Succeed()
        {
            _sut.With<FakeChainLink0>("01").Should().BeOfType<ChainFlowBuilder>();
        }

        [Fact]
        public void WithBooleanRouter_WhenRegisteredLinksArePassed_Succeed()
        {
            _sut.WithBooleanRouter<IRouterDispatcher<bool>>(
                (x) => x.With<FakeChainLink2>().Build(),
                (x) => x.With<FakeChainLink3>().Build()
            ).Should().BeOfType<ChainFlowBuilder>();
        }

        [Fact]
        public void WithBooleanRouter_WhenUnregisteredLinkIsPassedAsRightFlow_ThrowsException()
        {
            var act = () => _sut.WithBooleanRouter<IRouterDispatcher<bool>>(
                (x) => x.With<IChainFlow>().Build(),
                (x) => x.With<FakeChainLink3>().Build()
            );
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void WithBooleanRouter_WhenUnregisteredLinkIsPassedAsLeftFlow_ThrowsException()
        {
            var act = () => _sut.WithBooleanRouter<IRouterDispatcher<bool>>(
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
                .With<FakeChainLink0>("01")
                .Build();
            chain.Should().BeOfType<FakeChainLink1>();
        }

        [Fact]
        public void Build_WhenFlowsWithSuffixAreGiven_ReturnsFirstLink()
        {
            var chain =_sut
                .With<FakeChainLink0>()
                .Build();
            var chain1 = _sut
                .With<FakeChainLink0>("01")
                .Build();
            chain.Should().Be(ChainFlow0);
            chain1.Should().Be(ChainFlow001);
        }

        [Fact]
        public void Build_WhenDeclarationIsValidAlsoWithBooleanRouter_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .WithBooleanRouter<IRouterDispatcher<bool>>(
                    (x) => x.With<FakeChainLink2>().Build(),
                    (x) => x.With<FakeChainLink3>().Build()
                )
                .With<FakeChainLink0>("01")
                .Build();
            chain.Should().BeOfType<FakeChainLink1>();
        }

        [Fact]
        public void Build_WhenDeclarationIsValidAlsoWithNestedBooleanRouter_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .WithBooleanRouter<IRouterDispatcher<bool>>(
                    (x) => x
                        .With<FakeChainLink2>()
                        .WithBooleanRouter<IRouterDispatcher<bool>>(
                            y => y.With<FakeChainLink0>("01").Build(),
                            y => y.With<FakeChainLink3>().Build())
                        .Build(),
                    (x) => x.With<FakeChainLink3>().Build()
                )
                .Build();
            chain.Should().BeOfType<FakeChainLink1>();
        }
    }
}