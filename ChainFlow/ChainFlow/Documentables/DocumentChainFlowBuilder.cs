using ChainFlow.ChainFlows;
using ChainFlow.Enums;
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
        private bool _isMainBuilder = true;

        public DocumentChainFlowBuilder(IEnumerable<ChainFlowRegistration> links)
        {
            _links = links;
            _tags = new List<string>();
            _connections = new List<string>();
        }

        private DocumentChainFlowBuilder(IEnumerable<ChainFlowRegistration> links, bool isMainBuilder)
        {
            _links = links;
            _tags = new List<string>();
            _connections = new List<string>();
            _isMainBuilder = isMainBuilder;
        }

        public IChainFlow Build(FlowOutcome outcome)
        {
            if (!_tags.Any(x => x == GetOutcomeTagString(outcome)))
            {
                _tags.Add(GetOutcomeTagString(outcome));
            }

            foreach (ChainFlowRegistration current in _currentRegistration)
            {
                var flowId = current.GetDocumentFlowId();
                if (!_connections.Any(x => x.StartsWith(flowId)) && (_isMainBuilder || outcome != FlowOutcome.Success))
                {
                    _connections.Add($"{flowId} --> {outcome}");
                }
            }

            return _firstRegistration?.ChainLinkFactory()!;
        }

        private static string GetOutcomeTagString(FlowOutcome outcome)
        {
            return $"{outcome}(Workflow is completed with {(outcome == FlowOutcome.TransientFailure ? "transient failure" : outcome.ToString().ToLower())})";
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

            stringBuilder.AppendLine(string.Empty);

            foreach (var connection in _connections)
            {
                stringBuilder.AppendLine(connection.ToString());
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

            var rightBuilder = new DocumentChainFlowBuilder(_links, false);
            rightFlowFactory(rightBuilder);
            foreach (var t in rightBuilder._tags)
            {
                if (!_tags.Contains(t))
                {
                    _tags.Add(t);
                }
            }
            
            var leftBuilder = new DocumentChainFlowBuilder(_links, false);
            leftFlowFactory(leftBuilder);
            foreach (var t in leftBuilder._tags)
            {
                if (!_tags.Contains(t))
                {
                    _tags.Add(t);
                }
            }

            foreach (var current in _currentRegistration)
            {
                string connection = $"{current.GetDocumentFlowId()} --> {registration.GetDocumentFlowId()}";
                _connections.Add(connection);
            }

            string rightConnection = $"{registration.GetDocumentFlowId()} --True--> {rightBuilder._firstRegistration.GetDocumentFlowId()}";
            _connections.Add(rightConnection);
            string leftConnection = $"{registration.GetDocumentFlowId()} --False--> {leftBuilder._firstRegistration.GetDocumentFlowId()}";
            _connections.Add(leftConnection);

            foreach (var rightConn in rightBuilder._connections)
            {
                if (!_connections.Contains(rightConn))
                {
                    _connections.Add(rightConn);
                }
            }
            foreach (var leftConn in leftBuilder._connections)
            {
                if (!_connections.Contains(leftConn))
                {
                    _connections.Add(leftConn);
                }
            }

            _currentRegistration.Clear();
            _currentRegistration.Add(rightBuilder._currentRegistration.First());
            _currentRegistration.Add(leftBuilder._currentRegistration.First());

            return this;
        }
    }
}
