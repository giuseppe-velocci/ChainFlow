using ChainFlow.Enums;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChainFlowUnitTest")]
[assembly: InternalsVisibleTo("ChainFlow.TestKit")]
namespace ChainFlow.ChainBuilder
{
    internal sealed class ChainFlowBuilder : IChainFlowBuilder
    {
        private readonly IEnumerable<ChainFlowRegistration> _links;

        private IChainFlow _firstLink = null!;
        private IChainFlow _currentLink = null!;

        public ChainFlowBuilder(IEnumerable<ChainFlowRegistration> links)
        {
            if (links is null || !links.Any())
            {
                throw new ArgumentException($"ChainLinkRegistration list is {(links is null ? "null" : "empty")} so ChainBuilder cannot be resolved", nameof(links));
            }

            _links = links;
        }

        public IChainFlow Build(FlowOutcome outcome)
        {
            if (_currentLink is null)
            {
                throw new InvalidOperationException("Cannot resolve chain declaration");
            }

            var initialLink = _firstLink;
            _firstLink = null!;
            return initialLink;
        }

        public IChainFlowBuilder WithBooleanRouter<TRouterDispatcher>(
            Func<IChainFlowBuilder, IChainFlow> rightFlowFactory,
            Func<IChainFlowBuilder, IChainFlow> leftFlowFactory) where TRouterDispatcher : IRouterDispatcher<bool>
        {
            string dispatcherName = typeof(TRouterDispatcher).GetFullName();
            var rightFlow = rightFlowFactory(new ChainFlowBuilder(_links));
            var leftFlow = leftFlowFactory(new ChainFlowBuilder(_links));
            var resolvedLink = ((IBooleanRouterFlow)_links.First(x => x.ChainFlowName == dispatcherName).ChainLinkFactory())
                .WithRightFlow(rightFlow)
                .WithLeftFlow(leftFlow);
            return ReturnUpdatedBuilder(resolvedLink);
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            var resolvedLink = _links.First(x => x.ChainFlowName == typeof(T).GetFullName()).ChainLinkFactory();
            return ReturnUpdatedBuilder(resolvedLink);
        }

        public IChainFlowBuilder With<T>(string nameSuffix) where T : IChainFlow
        {
            var resolvedLink = _links.First(x => x.ChainFlowName == typeof(T).GetFullName(nameSuffix)).ChainLinkFactory();
            return ReturnUpdatedBuilder(resolvedLink);
        }

        private IChainFlowBuilder ReturnUpdatedBuilder(IChainFlow resolvedLink)
        {
            if (_firstLink is null)
            {
                _firstLink = resolvedLink;
                _currentLink = _firstLink;
            }
            else
            {
                _currentLink.SetNext(resolvedLink);
                _currentLink = resolvedLink;
            }

            return this;
        }
    }
}