﻿using ChainFlow.ChainBuilder;
using ChainFlow.ChainFlows;
using ChainFlow.Enums;
using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Internals;
using Microsoft.Extensions.Logging;

namespace ChainFlow.Debugger
{
    internal class DebugChainBuilder : IChainFlowBuilder
    {
        private readonly IChainFlowBuilder _builder;
        private readonly ILogger<DebugFlow> _logger;

        public DebugChainBuilder(IEnumerable<ChainFlowRegistration> links, ILogger<DebugFlow> logger)
        {
            _logger = logger;
            var debugLinks = links.Select(x =>
            {
                if (x.ChainLinkFactory().GetType().GetInterfaces().Any(x => x.Name is nameof(IBooleanRouterFlow)))
                {
                    IBooleanRouterFlow router = (IBooleanRouterFlow) x.ChainLinkFactory();
                    return new ChainFlowRegistration(x.ChainFlowName, () => new DebugBooleanRouterFlow(router, x.ChainFlowName, _logger));
                }
                else
                {
                    return new ChainFlowRegistration(x.ChainFlowName, () => new DebugFlow(x.ChainLinkFactory(), _logger));
                }
            });

            _builder = new ChainFlowBuilder(debugLinks);
        }

        public IChainFlow Build(FlowOutcome outcome = FlowOutcome.Success)
        {
            return _builder.Build(outcome);
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            return _builder.With<T>();
        }

        public IChainFlowBuilder With<T>(string nameSuffix) where T : IChainFlow
        {
            return _builder.With<T>(nameSuffix);
        }

        public IChainFlowBuilder WithBooleanRouter<TRouterDispatcher>(Func<IChainFlowBuilder, IChainFlow> rightFlowFactory, Func<IChainFlowBuilder, IChainFlow> leftFlowFactory) where TRouterDispatcher : IRouterDispatcher<bool>
        {
            return _builder.WithBooleanRouter<TRouterDispatcher>(rightFlowFactory, leftFlowFactory);
        }
    }
}