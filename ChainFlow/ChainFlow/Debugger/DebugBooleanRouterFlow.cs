using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using ChainFlow.Models;
using Microsoft.Extensions.Logging;

namespace ChainFlow.Debugger
{
    internal class DebugBooleanRouterFlow : IBooleanRouterFlow
    {
        private readonly IBooleanRouterFlow _chainFlow;
        private readonly ILogger<DebugFlow> _logger;
        private readonly string _chainFlowType;

        public DebugBooleanRouterFlow(IBooleanRouterFlow chainFlow, ILogger<DebugFlow> logger)
        {
            _chainFlow = chainFlow;
            _logger = logger;
            _chainFlowType = _chainFlow.GetType().GetFullName();
        }

        public string Describe()
        {
            return _chainFlow.Describe();
        }

        public async Task<ProcessingResult> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entering {flow} {id}", _chainFlowType, _chainFlow.GetHashCode());
            var result = await _chainFlow.ProcessAsync(message, cancellationToken);
            _logger.LogInformation("Leaving {flow} {id}", _chainFlowType, _chainFlow.GetHashCode());
            return result;
        }

        public void SetNext(IChainFlow next)
        {
            _chainFlow.SetNext(next);
        }

        public IBooleanRouterFlow WithLeftFlow(IChainFlow flow)
        {
            return _chainFlow.WithLeftFlow(flow);
        }

        public IBooleanRouterFlow WithRightFlow(IChainFlow flow)
        {
            return _chainFlow.WithRightFlow(flow);
        }
    }
}
    