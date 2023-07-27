﻿using ChainFlow.Interfaces.StorageFlowsDependencies;
using ChainFlow.Models;

namespace ChainFlow.ChainFlows.StorageFlows
{
    public class StorgeRemoverFlow<TIn> : AbstractChainFlow where TIn : class
    {
        protected readonly IStorageRemover<TIn> _remover;

        public StorgeRemoverFlow(IStorageRemover<TIn> remover)
        {
            _remover = remover;
        }

        public override string Describe() => $"Delete {typeof(TIn)} from storage";

        public async override Task<ProcessingRequestWithOutcome> ProcessRequestAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            OperationResult<bool> deletionResult = await _remover.RemoveAsync((TIn)message.Request, cancellationToken);
            return deletionResult.Value ?
                ProcessingRequestWithOutcome.CreateWithSuccess(message.Request) :
                ProcessingRequestWithOutcome.CreateWithFailure(message.Request, deletionResult.Message);
        }
    }
}