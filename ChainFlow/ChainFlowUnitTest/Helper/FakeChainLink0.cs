﻿using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlowUnitTest.Helper
{
    class FakeChainLink0 : IChainFlow
    {
        public virtual string Describe()
        {
            return "Do something @0";
        }

        public Task<ProcessingRequestWithOutcome> ProcessAsync(ProcessingRequest message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SetNext(IChainFlow next)
        {
            return;
        }
    }

    class FakeChainLink1 : FakeChainLink0 
    {
        public override string Describe()
        {
            return "Then do something else @1";
        }
    }

    class FakeChainLink2 : FakeChainLink0 
    {
        public override string Describe()
        {
            return "Then check something @2";
        }
    }

    class FakeChainLink3 : FakeChainLink0 
    {
        public override string Describe()
        {
            return "Then check something else @3";
        }
    }

    class FakeChainLink4 : FakeChainLink0 
    {
        public override string Describe()
        {
            return "Then again look for something else @4";
        }
    }
}