using ChainFlow.ChainBuilder;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.TestKit.Internals;
using FluentAssertions;
using Moq;

namespace ChainFlow.TestKit
{
    /// <summary>
    /// Class implementing IChainFlowBuilder able to create a DI Container for the declared Workflow and to keep track of
    /// a the call stak of IChainFlow executed by the Workflow under test
    /// </summary>
    public class Container
    {
        private readonly TestInstanceFactory _testInstanceFactory;
        private readonly List<ChainFlowRegistration> _links;
        private readonly List<string> _stack;
        private readonly TestKitChainFlowBuilder _builder;

        public Container()
        {
            _testInstanceFactory = new TestInstanceFactory(new MoqChainFlowDefaultValueProvider());
            _links = new List<ChainFlowRegistration>();
            _stack = new List<string>();
            _builder = new(_testInstanceFactory, _links, _stack);
        }

        /// <summary>
        /// Initialize an IChainFlowBuilder with all registered services to create a Workflow instace.
        /// It will scan the needed IChainFlow using a dedicated class impleneting IChainFlowBuilder. This process
        /// will create a DI container with all dependencies required, returning class instances whenever they are injected
        /// as explicit classes, and Mock<TInterface> when injected as interfaces. All created mocks can be retrieved and setup
        /// using GetMock<IMockedDependency>() method
        /// </summary>
        /// <param name="initializer">Action used to pass the constructor of a Workflow. Ex GetChainFlowBuilder((x) => new Workflow(x))</param>
        /// <returns>IChainFlowBuilder to be used to build the real Workflow sut instance</returns>
        public IChainFlowBuilder GetChainFlowBuilder(Action<IChainFlowBuilder> initializer)
        {
            initializer(_builder);
            return new ChainFlowBuilder(_links);
        }

        /// <summary>
        /// Return the Mock<IMockedDependency> instance from the list of auto-registered dependencies to allow setup
        /// for specific methods
        /// </summary>
        /// <typeparam name="IMockedDependency">Type of Mock<> to be returned. It must be an interface</typeparam>
        /// <returns>Mock<IMockedDependency> instance</returns>
        public Mock<IMockedDependency> GetMock<IMockedDependency>() where IMockedDependency : class
            => _testInstanceFactory.GetMock<IMockedDependency>();

        public void VerifyWorkflowCallStack(IEnumerable<ChainFlowStack> expectedChainFlowStack)
        {
            _stack.Should().BeEquivalentTo(expectedChainFlowStack.Select(x => x.Name));
        }
    }
}