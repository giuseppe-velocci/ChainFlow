# ChainFlow TestKit
A test kit for *ChainFlow* Workflows built on top of [Moq](https://github.com/devlooped/moq) (Sponsorlink-free version 4.20.69).
This library will make testing of Workflows a breeze.

## Installation
[Nuget package](https://www.nuget.org/packages/ChainFlow.TestKit)


## How to use
The usage is quite simple. Given a class injects an `IChainFlowBuilder`,the testKit can create a conctrete instance of the builder with all required dependencies needed to build the specified Workflow.
The entry point of the TestKit is the `Container` class:
```
Container container = new();
```

This container uses a fair simple rule. It scans, recursively, all constructors for the `IChainFlow`s declared in the Workflow and it will register:
 - if a class is found, a concrete instance of that class
 - if an interface is found, a `Mock<TInterface>` for that instance

Moreover, all registered `Mock<TInterface>` can be retrieved for further customization of the flow to be tested using method:
```
Mock<IMockedDependency> dependency = container.GetMock<IMockedDependency>();
```

This can be achieved with the inizialization method:
```
Container container = new();
IChainFlowBuilder builder = container.GetChainFlowBuilder((x) => new FakeWorkflow(x));
FakeWorkflow workflow = new(builder);
```

To ease developer experience, as long as a dependency returns an `OperationResult<T>` or a `bool`, the TestKit Container will return automatically a success result with a concrete instance of the declared type, so even complex flows can be tested in minutes.

After Worklow's `ProcessAsync()` method is executed in the test run, it can be checked which `IChainFlow`s were invoked by leveraging the following method:
```
container.VerifyWorkflowCallStack(new ChainFlowStack[]{});
```

The `ChainFlowStack` class is a wrapper used to point to a specific `IChainFlow` registration. The stack returned by the `Container` mantains the sequence in which IChainFlows have been processed, giving an exact view of the internal flow.
This class has 2 constructors, that match quite precisely the way an IChainFlow is registered in DI:
```
public ChainFlowStack(Type flowType);
public ChainFlowStack(Type flowType, string flowTag);
```

The second constructor, as per DI extension methods, is inteded to be used when multiple registration for the same `IChainFlow` type exists.

Here's a full example:
```
[Fact]
public async Task ProcessAsync_WhenMockIsSetupForFailure_ReturnsFailure()
{
    //Arrange
    Container container = new();
    IChainFlowBuilder builder = container.GetChainFlowBuilder((x) => new FakeWorkflow(x));
    FakeWorkflow workflow = new(builder);

    container.GetMock<IFakeDependency>()
        .Setup(x => x.ProcessAsync(It.IsAny<RequestToProcess>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(OperationResult<bool>.CreateWithFailure(false, string.Empty));

    //Act
    var result = await workflow.ProcessAsync(new RequestToProcess(new Input { Id = 1, Value = "abc" }), CancellationToken.None);

    //Assert
    result.Outcome.Should().Be(FlowOutcome.Success);
    container.VerifyWorkflowCallStack(new ChainFlowStack[]
    {
        new (typeof(DataValidatorFlow<Input>)),
        new (typeof(DataMapperFlow<Input, InputEnriched>)),
        new (typeof(StorageReaderFlow<InputEnriched, InputEnriched>)),
        new (typeof(InputFlowDispatcher)),
        new (typeof(StorageRemoverFlow<InputEnriched>)),
    });
}
```