using ChainFlow.ChainBuilder;
using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.TestKit.Internals;
using FluentAssertions;
using Moq;

namespace ChainFlow.TestKit
{
    public class Container : IChainFlowBuilder
    {
        private readonly TestInstanceFactory _testInstanceFactory;
        private readonly IList<ChainFlowRegistration> _links;
        private readonly IList<string> _stack;

        public Container()
        {
            _testInstanceFactory = new TestInstanceFactory(new MoqChainFlowDefaultValueProvider());
            _links = new List<ChainFlowRegistration>();
            _stack = new List<string>();
        }

        public IChainFlow Build(FlowOutcome outcome = FlowOutcome.Success)
        {
            return null!;
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            return With<T>(string.Empty);
        }

        public IChainFlowBuilder With<T>(string nameSuffix) where T : IChainFlow
        {
            object mockFlow = _testInstanceFactory.CreateInstance(typeof(T));
            _links.Add(new ChainFlowRegistration(typeof(T), () => new StackChainFlowDecorator((T)mockFlow, _stack, nameSuffix), nameSuffix));
            return this;
        }

        public IChainFlowBuilder WithBooleanRouter<TRouterDispatcher>(
            Func<IChainFlowBuilder, IChainFlow> rightFlowFactory,
            Func<IChainFlowBuilder, IChainFlow> leftFlowFactory
            ) where TRouterDispatcher : IRouterDispatcher<bool>
        {
            var _ = rightFlowFactory(this);
            var __ = leftFlowFactory(this);
            object mockDispatcher = _testInstanceFactory.CreateInstance(typeof(TRouterDispatcher));
            _links.Add(new ChainFlowRegistration(
                typeof(TRouterDispatcher),
                () => new StackBooleanRouterChainFlowDecorator((TRouterDispatcher)mockDispatcher, _stack))
            );
            return this;
        }

        public IChainFlowBuilder GetChainFlowBuilder<T>(Action<Container> initializer)
        {
            initializer(this);
            return new ChainFlowBuilder(_links);
        }

        public Mock<IMockedDependency> GetMock<IMockedDependency>() where IMockedDependency : class
            => _testInstanceFactory.GetMock<IMockedDependency>();

        public void VerifyWorkflowCallStack(IEnumerable<ChainFlowStack> expectedChainFlowStack)
        {
            _stack.Should().BeEquivalentTo(expectedChainFlowStack.Select(x => x.Name));
        }
    }
}