using ChainFlow.ChainFlows.DataFlows;
using ChainFlow.Enums;
using ChainFlow.Interfaces;
using ChainFlow.Models;
using ChainFlow.TestKit;
using Console;
using Console.Dispatchers;
using Console.Flows;
using Console.Validators;
using FluentAssertions;

namespace ConsoleTests
{
    public class ConsoleWorkflowTest
    {
        [Fact]
        public async Task ProcessAsync_WhenValidMessageIspassed_Success()
        {
            Container container = new ();
            IChainFlowBuilder builder = container.GetChainFlowBuilder((x) => { ConsoleWorkflow _ = new(x); });
            ConsoleWorkflow workflow = new(builder);

            var result = await workflow.ProcessAsync(new RequestToProcess("name"), CancellationToken.None);

            result.Outcome.Should().Be(FlowOutcome.Success);
            container.VerifyWorkflowCallStack(new ChainFlowStack[]
            {
                new (typeof(DataValidatorFlow<string>), nameof(StringValidator)),
                new (typeof(IsConsoleToTerminateDispatcher)),
                new(typeof(DataValidatorFlow<string>), nameof(NameValidator)),
                new(typeof(GreeterFlow))
            });
        }
    }
}