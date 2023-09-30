using ChainFlow.Helpers;

namespace ChainFlow.TestKit
{
    /// <summary>
    /// Class used to assert that tested Workflow executed the specified ChainFlow
    /// </summary>
    public class ChainFlowStack
    {
        public string Name { get; }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="flowType">Type of the flow to be evaluated</param>
        public ChainFlowStack(Type flowType)
        {
            Name = flowType.GetFullName();
        }

        /// <summary>
        /// Constructor to be used when multiple registration for the same IChainFlow Type exists
        /// </summary>
        /// <param name="flowType">Type of the flow to be evaluated</param>
        /// <param name="flowTag">Optional string with tag to resolve a specific instance in case of multiple 
        /// registrations for the same IChainFlow Type</param>
        public ChainFlowStack(Type flowType, string flowTag)
        {
            Name = flowType.GetFullName(flowTag);
        }
    }
}