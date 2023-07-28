using ChainFlow.ChainFlows;
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

        private ChainFlowRegistration _firstRegistration = null!;
        private IList<ChainFlowRegistration> _currentRegistration = new List<ChainFlowRegistration>();

        public DocumentChainFlowBuilder(IEnumerable<ChainFlowRegistration> links)
        {
            _links = links;
            _tags = new List<string>();
            _connections = new List<string>();
        }

        public IChainFlow Build()
        {
            return _firstRegistration?.ChainLinkFactory()!;
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            var registration = _links.First(x => x.LinkType == typeof(T).FullName);
            _firstRegistration ??= registration;

            string tag = $"{registration.GetDocumentFlowId()}({registration.ChainLinkFactory().Describe()})";
            _tags.Add(tag);

            foreach (var current in _currentRegistration)
            {
                string connection = $"{current.GetDocumentFlowId()} --> {registration.GetDocumentFlowId()}";
                _connections.Add(connection);
            }

            _currentRegistration.Clear();
            _currentRegistration.Add(registration);
            return this;
        }

        public override string ToString()
        {
            if (!_currentRegistration.Any()) 
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
            foreach (var current in _currentRegistration)
            {
                stringBuilder.AppendLine($"{current.GetDocumentFlowId()} --> Success");
            }
            stringBuilder.Append(":::");

            return stringBuilder.ToString();
        }

        public IChainFlowBuilder WithBooleanRouter<TRouter>(Func<IChainFlowBuilder, IChainFlow> rightFlowFactory, Func<IChainFlowBuilder, IChainFlow> leftFlowFactory) where TRouter : IRouterLogic<bool>
        {
            var registration = _links.First(x => x.LinkType == typeof(BooleanRouterFlow<TRouter>).FullName);
            _firstRegistration ??= registration;

            string tag = $"{registration.GetDocumentFlowId()}{{{registration.ChainLinkFactory().Describe()}}}";
            _tags.Add(tag);

            var rightBuilder = new DocumentChainFlowBuilder(_links);
            rightFlowFactory(rightBuilder);
            foreach (var t in rightBuilder._tags)
            {
                _tags.Add(t);
            }
            
            var leftBuilder = new DocumentChainFlowBuilder(_links);
            leftFlowFactory(leftBuilder);
            foreach (var t in leftBuilder._tags)
            {
                _tags.Add(t);
            }

            string rightConnection = $"{registration.GetDocumentFlowId()} --True--> {rightBuilder._firstRegistration.GetDocumentFlowId()}";
            _connections.Add(rightConnection);
            string leftConnection = $"{registration.GetDocumentFlowId()} --False--> {leftBuilder._firstRegistration.GetDocumentFlowId()}";
            _connections.Add(leftConnection);

            foreach (var rightConn in rightBuilder._connections)
            {
                _connections.Add(rightConn);
            }
            foreach (var leftConn in leftBuilder._connections)
            {
                _connections.Add(leftConn);
            }

            _currentRegistration.Clear();
            _currentRegistration.Add(rightBuilder._currentRegistration.First());
            _currentRegistration.Add(leftBuilder._currentRegistration.First());

            return this;
        }
    }
}
