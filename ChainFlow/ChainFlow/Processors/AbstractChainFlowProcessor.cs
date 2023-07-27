using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlow.Processors
{
    public abstract class AbstractChainFlowProcessor<T> : IProcessor<T> where T : notnull
    {
        private readonly IChainFlowBuilder _chainBuilder = null!;
        private IChainFlow chain = null!;

        public AbstractChainFlowProcessor(IChainFlowBuilder chainBuilder)
        {
            _chainBuilder = chainBuilder;
        }

        public abstract T Outcome2T(ProcessingRequestWithOutcome outcome);

        public virtual string Describe()
        {
            return null!;
        }

        public async Task<T> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            chain ??= _chainBuilder.Build();

            var response = await chain.ProcessAsync(message, cancellationToken);
            return Outcome2T(response);
        }
    }
}
