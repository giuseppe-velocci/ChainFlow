using ChainFlow.Interfaces;
using System.Text;

namespace ChainFlow.Documentables
{
    internal sealed class DocumentableProgram
    {
        private readonly IEnumerable<IDocumentableWorkflow> _workflows;
        private readonly ISystemIoWriter _fileSystem;

        public DocumentableProgram(IEnumerable<IDocumentableWorkflow> workflows, ISystemIoWriter fileSystem)
        {
            _workflows = workflows;
            _fileSystem = fileSystem;
        }

        public Task RunAsync(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var  workflow in _workflows)
            {
                // worflow name and description
                stringBuilder.AppendLine("##" + workflow.GetWorkflowName());
                stringBuilder.AppendLine(workflow.Describe());
                stringBuilder.AppendLine(string.Empty);

                // flowchart
                stringBuilder.AppendLine("::: mermaid");
                stringBuilder.AppendLine("graph TD;");
                stringBuilder.AppendLine(workflow.DescribeWorkflowEntryPoint());
                stringBuilder.AppendLine(workflow.GetFlow());
                stringBuilder.AppendLine(":::");
                stringBuilder.AppendLine(string.Empty);
            }

            return _fileSystem.WriteFile("graph.md", stringBuilder.ToString().Trim());
        }
    }
}
