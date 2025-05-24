using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.GUI.Helpers;
using Process_Explorer.GUI.Models;
using SkiaSharp;

namespace Process_Explorer.GUI.ViewModels
{
    public partial class MetricsViewModel : ObservableObject, IDisposable
    {
        private readonly ProcessMetricsHostedService _service;
        private readonly Timer _timer;
        private readonly ObservableCollection<double> _privateValues = new();
        private readonly ObservableCollection<double> _workingValues = new();

        public static ObservableCollection<MemorySize> MemorySizes { get; } = MemoryUsageChart.InitializeSizes();

        [ObservableProperty]
        private MemorySize _selectedMemVarPrivateBytes;

        [ObservableProperty]
        private MemorySize _selectedMemVarWorkingSet;

        private MemoryUsageChart _privateChart;
        private MemoryUsageChart _workingChart;

        public ObservableCollection<ISeries> PrivateBytesChartSeries => _privateChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> PrivateBytesXAxes => _privateChart.XAxes;
        public ObservableCollection<ICartesianAxis> PrivateBytesYAxes => _privateChart.YAxes;

        public ObservableCollection<ISeries> WorkingSetChartSeries => _workingChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> WorkingSetXAxes => _workingChart.XAxes;
        public ObservableCollection<ICartesianAxis> WorkingSetYAxes => _workingChart.YAxes;

        public MetricsViewModel(ProcessMetricsHostedService service)
        {
            _service = service;
            _selectedMemVarPrivateBytes = MemorySizes.Last();
            _selectedMemVarWorkingSet = MemorySizes.Last();

            _privateChart = new MemoryUsageChart(
                _privateValues,
                _selectedMemVarPrivateBytes,
                SKColors.Red,
                40);

            _workingChart = new MemoryUsageChart(
                _workingValues,
                _selectedMemVarWorkingSet,
                SKColors.Yellow,
                40);

            _timer = new Timer(UpdateMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateMetrics(object? state)
        {
            var procs = _service.processes;
            if (!procs.Any()) return;

            var sumP = procs.Sum(p => p.PrivateBytes) / SelectedMemVarPrivateBytes.Value;
            var sumW = procs.Sum(p => p.WorkingSet) / SelectedMemVarWorkingSet.Value;

            _privateValues.Add(sumP);
            _workingValues.Add(sumW);

            if (_privateValues.Count > 100) _privateValues.RemoveAt(0);
            if (_workingValues.Count > 100) _workingValues.RemoveAt(0);
        }

        partial void OnSelectedMemVarPrivateBytesChanged(MemorySize oldVal, MemorySize newVal)
        {
            _privateChart.SelectedMemorySize = newVal;
            OnPropertyChanged(nameof(PrivateBytesChartSeries));
            OnPropertyChanged(nameof(PrivateBytesXAxes));
            OnPropertyChanged(nameof(PrivateBytesYAxes));
        }

        partial void OnSelectedMemVarWorkingSetChanged(MemorySize oldVal, MemorySize newVal)
        {
            _workingChart.SelectedMemorySize = newVal;
            OnPropertyChanged(nameof(WorkingSetChartSeries));
            OnPropertyChanged(nameof(WorkingSetXAxes));
            OnPropertyChanged(nameof(WorkingSetYAxes));
        }

        public void Dispose() => _timer.Dispose();
    }
}