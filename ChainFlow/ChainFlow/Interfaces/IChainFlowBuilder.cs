using ChainFlow.Enums;

namespace ChainFlow.Interfaces
{
    public interface IChainFlowBuilder
    {
        /// <summary>
        /// Add an IChainFlow to the workflow
        /// </summary>
        /// <typeparam name="T">Concrete type of Flow. Must implement IChainFlow</typeparam>
        /// <returns>Current IChainFlowBuilder</returns>
        IChainFlowBuilder With<T>() where T : IChainFlow;

        /// <summary>
        /// Add an IChainFlow to the workflow
        /// </summary>
        /// <typeparam name="T">Concrete type of Flow. Must implement IChainFlow</typeparam>
        /// <param name="nameSuffix">Suffix used to identify a single IChainFlow instance among many of the same type</param>
        /// <returns>Current IChainFlowBuilder</returns>
        IChainFlowBuilder With<T>(string nameSuffix) where T : IChainFlow;

        /// <summary>
        /// Add a boolean router to the workflow that will dispatch incoming request to either one of its children flows
        /// </summary>
        /// <typeparam name="TRouterDispatcher">Type of dispatcher that will evaulate request returning a bool</typeparam>
        /// <param name="rightFlowFactory">Next flow for the request in case the dispatcher returns True</param>
        /// <param name="leftFlowFactory">Next flow for the request in case the dispatcher returns False</param>
        /// <returns>Current IChainFlowBuilder</returns>
        IChainFlowBuilder WithBooleanRouter<TRouterDispatcher>(
            Func<IChainFlowBuilder, IChainFlow> rightFlowFactory,
            Func<IChainFlowBuilder, IChainFlow> leftFlowFactory) where TRouterDispatcher : IRouterDispatcher<bool>;

        /// <summary>
        /// Build the entire workflow and return the very first IChainFlow as the entry point 
        /// </summary>
        /// <param name="outcome">FlowOutcome is used by documentation feature to define if processing of current branch leads to a Success, a Failure or a Transient Failure</param>
        /// <returns>IChainFlow entry point</returns>
        IChainFlow Build(FlowOutcome outcome = FlowOutcome.Success);
    }
}
