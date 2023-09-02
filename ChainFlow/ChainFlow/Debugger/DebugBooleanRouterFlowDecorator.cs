using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;
using Microsoft.Extensions.Logging;

namespace ChainFlow.Debugger
{
    internal class DebugBooleanRouterFlowDecorator : IBooleanRouterFlow
    {
        private IBooleanRouterFlow _chainFlow;
        private readonly ILogger<DebugFlowDecorator> _logger;
        private readonly string _chainFlowName;

        public DebugBooleanRouterFlowDecorator(IBooleanRouterFlow chainFlow, string dispatcherName, ILogger<DebugFlowDecorator> logger)
        {
            _chainFlow = chainFlow;
            _logger = logger;
            _chainFlowName = $"{_chainFlow.GetType().GetFullName()} {dispatcherName}";
        }

        public string Describe()
        {
            return _chainFlow.Describe();
        }

        public async Task<ProcessingResult> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entering {flow} {id}", _chainFlowName, _chainFlow.GetHashCode());
            var result = await _chainFlow.ProcessAsync(message, cancellationToken);
            _logger.LogInformation("Leaving {flow} {id}", _chainFlowName, _chainFlow.GetHashCode());
            return result;
        }

        public void SetNext(IChainFlow next)
        {
            _chainFlow.SetNext(next);
        }

        public IBooleanRouterFlow WithLeftFlow(IChainFlow flow)
        {
            _chainFlow = _chainFlow.WithLeftFlow(flow);
            return this;
        }

        public IBooleanRouterFlow WithRightFlow(IChainFlow flow)
        {
            _chainFlow = _chainFlow.WithRightFlow(flow);
            return this;
        }

        public override int GetHashCode()
        {
            return _chainFlow.GetHashCode();
        }
    }
}
