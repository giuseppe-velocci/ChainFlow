﻿using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChainFlowUnitTest")]
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

        public IChainFlow Build()
        {
            if (_currentLink is null)
            {
                throw new InvalidOperationException("Cannot resolve chain declaration");
            }

            return _firstLink;
        }

        public IChainFlowBuilder With<T>() where T : IChainFlow
        {
            var resolvedLink = _links.First(x => x.LinkType == typeof(T).FullName).ChainLinkFactory();

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