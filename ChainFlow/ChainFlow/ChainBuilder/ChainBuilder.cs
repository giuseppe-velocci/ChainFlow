using ChainFlow.Interfaces;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChainFlowUnitTest")]
namespace ChainFlow.ChainBuilder
{
    internal sealed class ChainBuilder : IChainBuilder
    {
        private readonly IEnumerable<ChainLinkRegistration> _links;
        private IChainLink _firstLink = null!;
        private IChainLink _currentLink = null!;

        public ChainBuilder(IEnumerable<ChainLinkRegistration> links)
        {
            if (links is null || !links.Any())
            {
                throw new ArgumentException($"ChainLinkRegistration list is {(links is null ? "null" : "empty")} so ChainBuilder cannot be resolved", nameof(links));
            }

            _links = links;
        }

        public IChainLink Build()
        {
            if (_currentLink is null)
            {
                throw new InvalidOperationException("Cannot resolve chain declaration");
            }

            return _firstLink;
        }

        public IChainBuilder With<T>() where T : IChainLink
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