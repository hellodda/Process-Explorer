using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Process_Explorer.BLL.Models;
using Process_Explorer.BLL.Services;

namespace Process_Explorer.BLL.HostedServices
{
    public class ProcessMetricsHostedService : IHostedService
    {
        private readonly ILogger<ProcessMetricsHostedService> _logger = default!;
        private readonly IProcessService _service = default!;
        private Timer _timer = default!;

        public List<ProcessInformationDTO> Processes { get; private set; } = new();

        public ProcessMetricsHostedService(IProcessService service, ILogger<ProcessMetricsHostedService> logger)
        {
            _service = service;
            _logger = logger;
        }

        private async void UpdateProcesses(object? state)
        {
            try
            {
                Processes = (await _service.GetProcessesInformationAsync()).ToList();

                _logger.LogInformation("Process list updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating process list.");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateProcesses, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.DisposeAsync();
            return Task.CompletedTask;
        }
    }
}
