namespace ChainFlow.Interfaces
{
    public interface IDocumentableWorkflow
    {
        /// <summary>
        /// Define the main tutile for current workflow to be displayed in its documentation section
        /// </summary>
        /// <returns>name of the workflow</returns>
        string GetWorkflowName();
        /// <summary>
        /// Define a brief summary with relevant information for the workflow overall logic and behavior or purpose
        /// </summary>
        /// <returns>brief overview of the workflow</returns>
        string Describe();
        /// <summary>
        /// Define the text to be displayed in flow graph at the entry point
        /// </summary>
        /// <returns>text of the first graph item in the flow</returns>
        string DescribeWorkflowEntryPoint();
        /// <summary>
        /// Returns the graph with application flow as defined by IChainFlowBuilder
        /// </summary>
        /// <returns>markdown graph with Mermaid syntax</returns>
        string GetFlow();
    }
}
