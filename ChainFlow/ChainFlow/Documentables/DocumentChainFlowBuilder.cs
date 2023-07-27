using ChainFlow.Interfaces;
using ChainFlow.Models;
using System.Text;

namespace ChainFlow.Documentables
{
    internal class DocumentChainFlowBuilder : IChainFlowBuilder
    {
        private readonly IEnumerable<ChainFlowRegistration> _links;
        private readonly IList<string> _tags;
        private readonly IList<string> _connections;
        private ChainFlowRegistration _currentRegistration;

        public DocumentChainFlowBuilder(IEnumerable<ChainFlowRegistration> links)
        {
            _links = links;
            _tags = new List<string>();
            _connections = new List<string>();
        }

        public IChainFlow Build()
        {
            return _links.First().ChainLinkFactory();
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            var registration = _links.First(x => x.LinkType == typeof(T).FullName);

            string tag = $"{registration.GetDocumentFlowId()}({registration.ChainLinkFactory().Describe()})";
            _tags.Add(tag);

            if (_currentRegistration is not null)
            {
                string connection = $"{_currentRegistration.GetDocumentFlowId()} --> {registration.GetDocumentFlowId()}";
                _connections.Add(connection);
            }

            _currentRegistration = registration;
            return this;
        }

        public override string ToString()
        {
            if (_currentRegistration is null) 
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("::: mermaid");
            stringBuilder.AppendLine("graph TD;");
            foreach (var tag in _tags)
            {
                stringBuilder.AppendLine(tag.ToString());
            }
            stringBuilder.AppendLine("Success(Workflow is completed with success)");

            stringBuilder.AppendLine(string.Empty);

            foreach (var connection in _connections)
            {
                stringBuilder.AppendLine(connection.ToString());
            }
            stringBuilder.AppendLine($"{_currentRegistration.GetDocumentFlowId()} --> Success");
            stringBuilder.Append(":::");

            return stringBuilder.ToString();
        }
    }
}
