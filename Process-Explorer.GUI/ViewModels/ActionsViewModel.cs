using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.GUI.Helpers;
using System.Collections.ObjectModel;
using System.Threading;
using SkiaSharp;
using Process_Explorer.GUI.Models;
using System;
using System.Linq;

namespace Process_Explorer.GUI.ViewModels
{
    public class ActionsViewModel
    {
        private readonly ProcessMetricsHostedService _service;
        private readonly Timer _timer;
        private readonly ObservableCollection<double> _privateValues = new();

        private MemoryUsageChart _cpuChart;

        public ObservableCollection<ISeries> CpuChartSeries => _cpuChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> CpuXAxes => _cpuChart.XAxes;
        public ObservableCollection<ICartesianAxis> CpuYAxes => _cpuChart.YAxes;

        public ActionsViewModel(ProcessMetricsHostedService service)
        {
            _service = service;
            _cpuChart = new MemoryUsageChart(
               _privateValues,
               new MemorySize("cpu", 100),
               SKColors.MistyRose,
               40);

            _timer = new Timer(UpdateMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateMetrics(object? state)
        {
            var procs = _service.processes;
            if (!procs.Any()) return;

            var sumP = procs.Sum(p => p.PrivateBytes);

            _privateValues.Add(sumP);

            if (_privateValues.Count > 100) _privateValues.RemoveAt(0);
        }
    }
}
