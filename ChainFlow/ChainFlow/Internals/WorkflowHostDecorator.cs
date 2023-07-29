using ChainFlow.Documentables;
using Microsoft.Extensions.Hosting;

namespace ChainFlow.Internals
{
    internal class WorkflowHostDecorator : IHost
    {
        private readonly IHost _host;
        private readonly RunMode _runMode;

        public WorkflowHostDecorator(IHost host, RunMode runMode)
        {
            _host = host;
            _runMode = runMode;
        }

        public IServiceProvider Services => _host.Services;

        public void Dispose()
        {
            _host?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_runMode is RunMode.Documentation)
            {
                DocumentableProgram program = (DocumentableProgram)_host.Services.GetService(typeof(DocumentableProgram))!;
                return program.RunAsync();
            }
            else
            {
                return _host.StartAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return _host.StopAsync(cancellationToken);
        }
    }
}
