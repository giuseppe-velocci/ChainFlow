# ChainFlow
A Chain of Reponsibility centered library for Dotnet. 

## How does it work?
ChainFlow allows translating a flowchart in a code-equivalent version, and printing a markdown file (powered by Mermaid) to view an exact representation of the entire workflow so defined.

For example, given the following chart:

::: mermaid
graph TD;

:::

It can be traslated to a chain declaration:


Given that IHostBuilder has been initialized with the extension method:
```
```

If the program is run with --doc argument flag, it will write a markdown file:

::: mermaid
graph TD;

:::

Adding the registrations of the flows with all their dependencies in DI will further refine the produced flowchart outcome:



Features:
 - chain of responsibility builder to ease definition of workflows
 - built-in self documenting program mode
 - conditional flow routers to express even more clearly workflow
 - extensible basic flows ready to use
 - simple dependency interfaces to be implemented
