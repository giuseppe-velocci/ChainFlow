using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Internals;

namespace ChainFlow.TestKit.Internals
{
    internal class TestKitChainFlowBuilder : IChainFlowBuilder
    {
        private readonly TestInstanceFactory _testInstanceFactory;
        private readonly IList<ChainFlowRegistration> _links;
        private readonly IList<string> _stack;

        public TestKitChainFlowBuilder(
            TestInstanceFactory testInstanceFactory,
            IList<ChainFlowRegistration> links,
            IList<string> stack)
        {
            _testInstanceFactory = testInstanceFactory;
            _links = links;
            _stack = stack;
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

    }
}
