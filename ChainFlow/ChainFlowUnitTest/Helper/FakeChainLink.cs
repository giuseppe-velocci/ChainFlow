using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlowUnitTest.Helper
{
    class FakeChainLink : IChainFlow
    {
        public string Describe()
        {
            return "Do something";
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

    class FakeChainLink2 : FakeChainLink 
    {
        public new string Describe()
        {
            return "Then do something else";
        }
    }

    class FakeChainLink3 : FakeChainLink 
    {
        public new string Describe()
        {
            return "Then check something";
        }
    }

    class FakeChainLink4 : FakeChainLink 
    {
        public new string Describe()
        {
            return "Then check something else";
        }
    }

    class FakeChainLink5 : FakeChainLink 
    {
        public new string Describe()
        {
            return "Then again look for something else";
        }
    }
}