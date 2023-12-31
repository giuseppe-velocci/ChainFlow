﻿using ChainFlow.Interfaces;
using ChainFlow.Models;

namespace ChainFlowUnitTest.TestHelpers
{
    class FakeChainLink0 : IChainFlow
    {
        public virtual string Describe()
        {
            return "Do something @0";
        }

        public Task<ProcessingResult> ProcessAsync(RequestToProcess message, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ProcessingResult(message, ChainFlow.Enums.FlowOutcome.Success, Describe()));
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

internal static class ChainFlowExtension
{
    public static void ShouldBeEqual(this IChainFlow flow1, IChainFlow flow)
    {
        Assert.Equal(flow1.GetHashCode(), flow.GetHashCode());
    }
}