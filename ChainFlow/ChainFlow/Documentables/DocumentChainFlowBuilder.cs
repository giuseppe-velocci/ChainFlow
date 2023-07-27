using ChainFlow.ChainFlows;
using ChainFlow.Enums;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using System.Reflection;
using System.Text;

namespace ChainFlow.Documentables
{
    internal class DocumentChainFlowBuilder : IChainFlowBuilder
    {
        private readonly IEnumerable<ChainFlowRegistration> _links;
        private readonly IList<string> _tags;
        private readonly IList<string> _connections;

        private ChainFlowRegistration _firstRegistration = null!;
        private readonly IList<ChainFlowRegistrationWithBehavior> _currentRegistration = new List<ChainFlowRegistrationWithBehavior>();
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

            foreach (var current in _currentRegistration)
            {
                var flowId = current.GetDocumentFlowId();
                if (IsBehaviorNonStandard(current.Behavior) || IsOutcomeToBeAdded(outcome, flowId))
                {
                    _connections.Add(IsBehaviorNonStandard(current.Behavior) ?
                        $"{flowId} --True--> {outcome}" :
                        $"{flowId} --> {outcome}");
                }
            }

            return _firstRegistration?.ChainLinkFactory()!;
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            var registration = _links.FirstOrDefault(x => x.LinkType == typeof(T).GetFullName())
                ?? new ChainFlowRegistration(type: typeof(T), () => new TodoChainFlow(typeof(T)));
            _firstRegistration ??= registration;

            string boxStart = "(";
            string boxEnd = ")";
            var behavior = typeof(T).GetCustomAttribute<DocumentFlowBehaviorAttribute>();
            if (IsBehaviorNonStandard(behavior?.Behavior))
            {
                boxStart = "{";
                boxEnd = "}";

                var failureOutcome = behavior!.Behavior is DocumentFlowBehavior.TerminateOnFailure ?
                    FlowOutcome.Failure :
                    FlowOutcome.TransientFailure;
                _tags.Add(GetOutcomeTagString(failureOutcome));

                _connections.Add($"{registration.GetDocumentFlowId()} --False--> {failureOutcome}");
            }

            string tag = $"{registration.GetDocumentFlowId()}{boxStart}{registration.ChainLinkFactory().Describe()}{boxEnd}";
            _tags.Add(tag);

            AddCurrentConnections(registration);

            _currentRegistration.Clear();
            _currentRegistration.Add(new ChainFlowRegistrationWithBehavior(registration, behavior));
            return this;
        }

        public IChainFlowBuilder WithBooleanRouter<TRouter>(Func<IChainFlowBuilder, IChainFlow> rightFlowFactory, Func<IChainFlowBuilder, IChainFlow> leftFlowFactory) where TRouter : IRouterDispatcher<bool>
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

        public override string ToString()
        {
            if (!_currentRegistration.Any())
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            foreach (var tag in _tags.Where(x => x.StartsWith("_")))
            {
                stringBuilder.AppendLine(tag.ToString());
            }
            foreach (var tag in _tags.Where(x => !x.StartsWith("_")))
            {
                stringBuilder.AppendLine(tag.ToString());
            }

            stringBuilder.AppendLine(string.Empty);

            foreach (var connection in _connections)
            {
                stringBuilder.AppendLine(connection.ToString());
            }

            return stringBuilder.ToString().Trim();
        }

        private static bool IsBehaviorNonStandard(DocumentFlowBehavior? behavior)
        {
            return behavior is not null && behavior is not DocumentFlowBehavior.Standard;
        }

        private bool IsOutcomeToBeAdded(FlowOutcome outcome, string flowId)
        {
            return !_connections.Any(x => x.StartsWith(flowId)) && (_isMainBuilder || outcome != FlowOutcome.Success);
        }

        private static string GetOutcomeTagString(FlowOutcome outcome)
        {
            return $"{outcome}(Workflow is completed with {(outcome == FlowOutcome.TransientFailure ? "transient failure" : outcome.ToString().ToLower())})";
        }

        private void AddConnectionsForBuilder(DocumentChainFlowBuilder builder)
        {
            foreach (var conn in builder._connections.Where(conn => !_connections.Contains(conn)))
            {
                _connections.Add(conn);
            }
        }

        private void AddTagsDistinct(DocumentChainFlowBuilder builder)
        {
            foreach (var t in builder._tags.Where(t => !_tags.Contains(t)))
            {
                _tags.Add(t);
            }
        }

        private DocumentChainFlowBuilder GetEnrichedBuilder(Func<IChainFlowBuilder, IChainFlow> flowFactory)
        {
            var builder = new DocumentChainFlowBuilder(_links, false);
            flowFactory(builder);
            return builder;
        }

        private void AddCurrentConnections(ChainFlowRegistration registration)
        {
            foreach (var current in _currentRegistration)
            {
                string arrow = IsBehaviorNonStandard(current.Behavior) ?
                    "--True-->" :
                    "-->";
                string connection = $"{current.GetDocumentFlowId()} {arrow} {registration.GetDocumentFlowId()}";
                _connections.Add(connection);
            }
        }
    }
}
