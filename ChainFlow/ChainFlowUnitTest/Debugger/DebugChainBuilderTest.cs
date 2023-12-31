﻿using ChainFlow.ChainBuilder;
using ChainFlow.ChainFlows;
using ChainFlow.Debugger;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlowUnitTest.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChainFlowUnitTest.Debugger
{
    public class DebugChainBuilderTest
    {
        private readonly DebugChainBuilder _sut;
        private static readonly IChainFlow ChainFlow0 = new FakeChainLink0();
        private static readonly IChainFlow ChainFlow001 = new FakeChainLink0();
        private static readonly IChainFlow ChainFlow1 = new FakeChainLink1();
        private readonly ChainFlowRegistration[] _registrations = new ChainFlowRegistration[] {
            new ChainFlowRegistration(typeof(FakeChainLink0), () => ChainFlow0),
            new ChainFlowRegistration(typeof(FakeChainLink0), () => ChainFlow001, "01"),
            new ChainFlowRegistration(typeof(FakeChainLink1), () => ChainFlow1),
            new ChainFlowRegistration(typeof(FakeChainLink2), () => new FakeChainLink2()),
            new ChainFlowRegistration(typeof(FakeChainLink3), () => new FakeChainLink3()),
            new ChainFlowRegistration(typeof(FakeChainLink4), () => new FakeChainLink4()),
            new ChainFlowRegistration(typeof(IRouterDispatcher<bool>), () => new BooleanRouterFlow(new Mock<IRouterDispatcher<bool>>().Object)),
        };

        public DebugChainBuilderTest()
        {
            Mock<ILogger<DebugFlowDecorator>> mockLogger = new();
            _sut = new DebugChainBuilder(_registrations, mockLogger.Object);
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
        public void Build_WhenDeclarationIsValid_ReturnsFirstLinkWrappedInDebugFlow()
        {
            var chain = _sut
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .With<FakeChainLink0>("01")
                .Build();
            chain.Should().BeOfType<DebugFlowDecorator>();
            chain.ShouldBeEqual(ChainFlow1);
        }

        [Fact]
        public void Build_WhenFlowsWithSuffixAreGiven_ReturnsFirstLink()
        {
            var chain = _sut
                .With<FakeChainLink0>()
                .Build();
            var chain1 = _sut
                .With<FakeChainLink0>("01")
                .Build();
            chain.ShouldBeEqual(ChainFlow0);
            chain1.ShouldBeEqual(ChainFlow001);
        }

        [Fact]
        public void Build_WhenDeclarationIsValidAlsoWithBooleanRouter_ReturnsDebugBooleanRouterFlow()
        {
            var chain = _sut
                .WithBooleanRouter<IRouterDispatcher<bool>>(
                    (x) => x.With<FakeChainLink2>().Build(),
                    (x) => x.With<FakeChainLink3>().Build()
                )
                .With<FakeChainLink1>()
                .With<FakeChainLink0>()
                .With<FakeChainLink0>("01")
                .Build();
            chain.Should().BeOfType<DebugBooleanRouterFlowDecorator>();
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
            chain.Should().BeOfType<DebugFlowDecorator>();
            chain.ShouldBeEqual(ChainFlow1);
        }
    }
}
