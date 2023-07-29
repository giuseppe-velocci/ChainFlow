using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChainFlow.Internals
{
    internal class WorkflowHostBuilderDecorator : IHostBuilder
    {
        private readonly IHostBuilder _hostBuilder;
        private readonly RunMode _runMode;

        public WorkflowHostBuilderDecorator(IHostBuilder hostBuilder, RunMode runMode)
        {
            _hostBuilder = hostBuilder;
            _runMode = runMode;
        }

        public IDictionary<object, object> Properties => _hostBuilder.Properties;

        public IHost Build()
        {
            return new WorkflowHostDecorator(_hostBuilder.Build(), _runMode);
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return new WorkflowHostBuilderDecorator(_hostBuilder.ConfigureAppConfiguration(configureDelegate), _runMode);
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            return new WorkflowHostBuilderDecorator(_hostBuilder.ConfigureContainer<TContainerBuilder>(configureDelegate), _runMode);
        }

        public IHostBuilder ConfigureHostConfiguration(Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configureDelegate)
        {
            return new WorkflowHostBuilderDecorator(_hostBuilder.ConfigureHostConfiguration(configureDelegate), _runMode);
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            return new WorkflowHostBuilderDecorator(_hostBuilder.ConfigureServices(configureDelegate), _runMode);
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
        {
            return new WorkflowHostBuilderDecorator(_hostBuilder.UseServiceProviderFactory<TContainerBuilder>(factory), _runMode);
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
        {
            return new WorkflowHostBuilderDecorator(_hostBuilder.UseServiceProviderFactory(factory), _runMode);
        }
    }
}
