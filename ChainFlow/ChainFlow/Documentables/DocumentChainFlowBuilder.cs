using ChainFlow.ChainFlows;
using ChainFlow.Enums;
using ChainFlow.Helpers;
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
        private readonly IList<ChainFlowRegistration> _currentRegistration = new List<ChainFlowRegistration>();
        private readonly bool _isMainBuilder = true;

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
                if (IsOutcomeToBeAdded(outcome, flowId))
                {
                    _connections.Add($"{flowId} --> {outcome}");
                }
            }

            return _firstRegistration?.ChainLinkFactory()!;
        }

        private bool IsOutcomeToBeAdded(FlowOutcome outcome, string flowId)
        {
            return !_connections.Any(x => x.StartsWith(flowId)) && (_isMainBuilder || outcome != FlowOutcome.Success);
        }

        private static string GetOutcomeTagString(FlowOutcome outcome)
        {
            return $"{outcome}(Workflow is completed with {(outcome == FlowOutcome.TransientFailure ? "transient failure" : outcome.ToString().ToLower())})";
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            var registration = _links.FirstOrDefault(x => x.LinkType == typeof(T).GetFullName())
                ?? new ChainFlowRegistration(type: typeof(T), () => new TodoChainFlow(typeof(T)));
            _firstRegistration ??= registration;

            string tag = $"{registration.GetDocumentFlowId()}({registration.ChainLinkFactory().Describe()})";
            _tags.Add(tag);

            AddCurrentConnections(registration);

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
            var registration = _links.FirstOrDefault(x => x.LinkType == typeof(BooleanRouterFlow<TRouter>).GetFullName())
                ?? new ChainFlowRegistration(typeof(TRouter), () => new TodoBooleanRouterChainFlow<TRouter>(default!));
            _firstRegistration ??= registration;

            string tag = $"{registration.GetDocumentFlowId()}{{{registration.ChainLinkFactory().Describe()}}}";
            _tags.Add(tag);
            DocumentChainFlowBuilder rightBuilder = GetEnrichedBuilder(rightFlowFactory);
            AddTagsDistinct(rightBuilder);

            DocumentChainFlowBuilder leftBuilder = GetEnrichedBuilder(leftFlowFactory);
            AddTagsDistinct(leftBuilder);

            AddCurrentConnections(registration);

            string rightConnection = $"{registration.GetDocumentFlowId()} --True--> {rightBuilder._firstRegistration.GetDocumentFlowId()}";
            _connections.Add(rightConnection);
            string leftConnection = $"{registration.GetDocumentFlowId()} --False--> {leftBuilder._firstRegistration.GetDocumentFlowId()}";
            _connections.Add(leftConnection);

            AddConnectionsForBuilder(rightBuilder);

            AddConnectionsForBuilder(leftBuilder);

            _currentRegistration.Clear();
            _currentRegistration.Add(rightBuilder._currentRegistration.First());
            _currentRegistration.Add(leftBuilder._currentRegistration.First());

            return this;
        }

        private void AddConnectionsForBuilder(DocumentChainFlowBuilder builder)
        {
            foreach (var conn in builder._connections.Where(conn => !_connections.Contains(conn)))
            {
                _connections.Add(conn);
            }
        }

        private void AddTagsDistinct(DocumentChainFlowBuilder rightBuilder)
        {
            foreach (var t in rightBuilder._tags.Where(t => !_tags.Contains(t)))
            {
                _tags.Add(t);
            }
        }

        private DocumentChainFlowBuilder GetEnrichedBuilder(Func<IChainFlowBuilder, IChainFlow> rightFlowFactory)
        {
            var rightBuilder = new DocumentChainFlowBuilder(_links, false);
            rightFlowFactory(rightBuilder);
            return rightBuilder;
        }

        private void AddCurrentConnections(ChainFlowRegistration registration)
        {
            foreach (var current in _currentRegistration)
            {
                string connection = $"{current.GetDocumentFlowId()} --> {registration.GetDocumentFlowId()}";
                _connections.Add(connection);
            }
        }
    }

    class TodoChainFlow : IChainFlow
    {
        private readonly string _description;
        public TodoChainFlow(Type type) 
        { 
            _description = $"TODO {type.GetFullName()}";
        }

        public string Describe() => _description;

        public Task<ProcessingRequestWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
            => Task.FromResult((ProcessingRequestWithOutcome)null!);

        public void SetNext(IChainFlow next) { }
    }

    class TodoBooleanRouterChainFlow<T> : BooleanRouterFlow<T>, IChainFlow where T : IRouterLogic<bool>
    {
        private readonly string _description;
        public TodoBooleanRouterChainFlow(T routerLogic) : base(routerLogic)
        { 
            _description = $"TODO RouterFlow {typeof(T).GetFullName()}";
        }

        public override string Describe() => _description;
    }
}
