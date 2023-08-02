using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ChainFlow.Documentables
{
    internal sealed class DocumentableProgram
    {
        private readonly IEnumerable<IDocumentableWorkflow> _workflows;
        private readonly ISystemIoWriter _fileSystem;
        private readonly ILogger<DocumentableProgram> _logger;

        public DocumentableProgram(
            IEnumerable<IDocumentableWorkflow> workflows,
            ISystemIoWriter fileSystem,
            ILogger<DocumentableProgram> logger)
        {
            _workflows = workflows;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Starting document generation...");

            foreach (var workflow in _workflows)
            {
                StringBuilder stringBuilder = new();

                // worflow name and description
                stringBuilder.AppendLine($"## {workflow.GetWorkflowName()}");
                stringBuilder.AppendLine(workflow.Describe());
                stringBuilder.AppendLine(string.Empty);

                // flowchart
                stringBuilder.AppendLine("::: mermaid");
                stringBuilder.AppendLine("graph TD;");
                stringBuilder.AppendLine($"_start({workflow.DescribeWorkflowEntryPoint()}) -->");
                stringBuilder.AppendLine(workflow.GetFlow());
                stringBuilder.AppendLine(":::");
                stringBuilder.AppendLine(string.Empty);

                // store one file for each workflow
                string filename = $"{FilenameSanitizer.Sanitize(workflow.GetWorkflowName() ?? "graph")}.md";
                _logger.LogInformation("Writing document {doc}...", filename);
                await _fileSystem.WriteFile(filename, stringBuilder.ToString().Trim());
                _logger.LogInformation("Created document {doc}", filename);
            }
            _logger.LogInformation("Document generation complete");
        }
    }
}
