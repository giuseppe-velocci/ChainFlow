# ChainFlow
A modern framework for Dotnet that blends Chain of Reponsibility with Builder pattern.
This blend allows a fluent declarative way to define even complex workflows, keeping intact all advantages given by those patterns.

## Features:
 - chain of responsibility declarative definition to ease creation of workflows
 - built-in self documenting program mode
 - built-in debugging mode to log which ChainFlow is currently handlying data
 - conditional flow routers to express workflows in a clear way
 - extensible basic flows ready to use
 - simple dependency interfaces to be implemented

 ## Benefits
 Core benefits are mainly inherited by Chain of Responsibility pattern, with some addition:
- promotion of single responsibility classes
- clear and predictable approach to add/remove steps in a workflow
- clear codebase structure
- reusability of components
- dependency injection optimized for testing
- easily testable with ChainFlow.TestKit package

## How does it work?
**ChainFlow** allows translating a flowchart in a code-equivalent version, and printing a markdown file (powered by [Mermaid](https://mermaid.js.org/)) to view an exact representation of the entire workflow.

For example, given the following chart:

![Diagramma senza titolo drawio](https://github.com/giuseppe-velocci/ChainFlow/assets/42746122/15d61f74-1072-4923-a1a6-ecdb8b5f88aa)

It can be traslated to a chain declaration inside the constructor of a Worflow class:
```
class ConsoleWorkflow : AbstractWorkflowRunner, IHostedService
{
    public ConsoleWorkflow(IChainFlowBuilder chainBuilder) : base(chainBuilder)
    {
        Workflow = chainBuilder
            .With<DataValidatorFlow<string>>(nameof(StringValidator))
            .WithBooleanRouter<IsConsoleToTerminateDispatcher>(
                (x) => x
                    .With<TerminateConsoleFlow>()
                    .Build(),
                (x) => x
                    .With<DataValidatorFlow<string>>(nameof(NameValidator))
                    .With<GreeterFlow>()
                    .Build()
            )
            .Build();
    }
}
```

Then, initialize `IHostBuilder` with the provided extension method:

```
var host = Host
        .CreateDefaultBuilder(args)
        .InitializeWorkflowHostBuilder(args) // this is needed to initialize ChainFlow
```

and setup DI to register ChainFlows:

```
.ConfigureServices((hostContext, services) =>
{
    services
        // register ChainFlows
        .AddChainFlow<GreeterFlow>()
        .AddChainFlow<TerminateConsoleFlow>()
        .AddBooleanRouterChainFlow<IsConsoleToTerminateDispatcher>()

        // register 2 concrete instances of DataValidatorFlow<string> with a tag to let DI identify them
        .AddChainFlow((sp) => new DataValidatorFlow<string>(sp.GetRequiredService<StringValidator>()), nameof(StringValidator))
        .AddChainFlow((sp) => new DataValidatorFlow<string>(sp.GetRequiredService<NameValidator>()), nameof(NameValidator))

        // register dependencies    
        .AddSingleton<StringValidator>()
        .AddSingleton<NameValidator>()
        
        // register main hosted service
        .AddHostedService<ConsoleWorkflow>();
});
```

Now the program is enabled to use full features from **ChainFlow**.

## Run modes
It can be executed in 3 diffrent modes, depending on argument passed to execution:
- no argument: runs business logic as expected
- `--debug`: runs business logic with additional logs detailing which IChainFlow is handling data
- `--doc`: runs alternate Autodocumentation mode that creates a *markdown* file with a [Mermaid](https://mermaid.js.org/) graph describing the flow

## Autodocumentation
**ChainFlow** brings documentation literally inside the codebase.
There are 2 main sources for these information to be retrieved:
- Workflows implementing `IDocumentableWorkflow` that declare:
  - Name of the workflow
  - A description of the workflow, that can be enriched using md syntax
- `IChainFlow` instances where diagram boxes take their labels

When starting the program with `--doc` flag, it will output a markdown file ready to become part of documentation for the team.

This tool can also be used when DI is not yet fully implemented. The bare minimum for it to work is to initialize the framework and register an existing workflow:
```
var host = Host
        .CreateDefaultBuilder(args)
        .InitializeWorkflowHostBuilder(args) // this is needed to initialize ChainFlow
        .ConfigureServices((hostContext, services) =>
        {
            services
            .AddHostedService<FooWorkflow>();
        });
```

This will output a graph with unregistered flows marked with a name starting with TODO. Running the program in autodocumentation mode during development can become a powerful way to assess the project's status and areas not yet completed, or at least not correctly registered in DI and to get an initial overview of the data flow inside the system.

Adding the registrations of the flows with all their dependencies in DI will further refine the flowchart outcome.
Here an example of an output markdown file for the above service definition followed by a preview image:
```
## ConsoleWorkflow
A greeter console app with ChainFlow

::: mermaid
graph TD;
_start(When user input is received) -->
_1339362678{Is String valid?}
_1507651249{Has user terminated input sequence?}
_153473101(Exit program)
_631627724{Is String valid?}
_1530896123(Greet user by name)
Failure(Workflow is completed with failure)
Success(Workflow is completed with success)

_1339362678 --False--> Failure
_1339362678 --True--> _1507651249
_1507651249 --True--> _153473101
_1507651249 --False--> _631627724
_631627724 --False--> Failure
_631627724 --True--> _1530896123
_153473101 --> Success
_1530896123 --> Success
:::
```
![immagine](https://github.com/giuseppe-velocci/ChainFlow/assets/42746122/a2e5cf73-322c-467c-a7fb-fa6c5edbbc76)


