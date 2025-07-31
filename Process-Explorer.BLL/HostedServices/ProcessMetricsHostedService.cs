using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Process_Explorer.BLL.Models;
using Process_Explorer.BLL.Services;

namespace Process_Explorer.BLL.HostedServices
{
    public class ProcessMetricsHostedService : BackgroundService
    {
        private readonly ILogger<ProcessMetricsHostedService> _logger;
        private readonly IProcessService _service;
        private Timer _timer = default!;

        public event Action<IReadOnlyList<ProcessInformationDTO>>? ProcessesUpdated;

        public ProcessMetricsHostedService(IProcessService service, ILogger<ProcessMetricsHostedService> logger)
        {
            _service = service;
            _logger = logger;
        }

        private async void UpdateProcesses(object? state)
        {
            try
            {
                var processes = (await _service.GetProcessesInformationAsync())!.ToList()!;

                ProcessesUpdated?.Invoke(processes!);

                _logger.LogInformation("Process list updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating process list.");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(UpdateProcesses, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }
    }

}