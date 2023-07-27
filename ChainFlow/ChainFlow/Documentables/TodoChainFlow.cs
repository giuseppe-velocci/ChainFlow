using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.Documentables
{
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
}
