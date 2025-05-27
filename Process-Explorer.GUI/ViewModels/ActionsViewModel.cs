using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.GUI.Helpers;
using System.Collections.ObjectModel;
using System.Threading;
using SkiaSharp;
using Process_Explorer.GUI.Models;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using Process_Explorer.BLL.Models;
using Microsoft.UI.Xaml;

namespace Process_Explorer.GUI.ViewModels
{
    ///------------------------------------------------
    /// 
    /// Skoro...
    /// 
    ///------------------------------------------------
    public partial class ActionsViewModel : ObservableObject
    {
        private readonly ProcessMetricsHostedService _service = default!;
        private readonly Timer _timer = default!;

        private readonly ObservableCollection<double> _cpuUsageValues = new();
        private readonly ObservableCollection<double> _privateBytesValues = new();
        private readonly ObservableCollection<double> _workingSetValues = new();

        private readonly DispatcherQueue _dispatcher = default!;

        private MemoryUsageChart _cpuChart;
        private MemoryUsageChart _privateBytesChart;
        private MemoryUsageChart _workingSetChart;

        [ObservableProperty]
        private Visibility _progressBarVisiblity;

        [ObservableProperty]
        private string _processAddress;

        [ObservableProperty]
        private bool _isLoading = true;

        private int _targetProcessId;
        public int TargetProcessId
        {
            get => _targetProcessId;
            set
            {
                if (SetProperty(ref _targetProcessId, value))
                {
                    IsLoading = true;
                    ProcessAddress = "Loading...";

                    _cpuChart.Reinitialize();
                    _privateBytesChart.Reinitialize();
                    _workingSetChart.Reinitialize();
                }
            }
        }

        public IRelayCommand KillCommand { get; } 

        public ObservableCollection<ISeries> CpuChartSeries => _cpuChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> CpuXAxes => _cpuChart.XAxes;
        public ObservableCollection<ICartesianAxis> CpuYAxes => _cpuChart.YAxes;

        public ObservableCollection<ISeries> PrivateBytesChartSeries => _privateBytesChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> PrivateBytesXAxes => _privateBytesChart.XAxes;
        public ObservableCollection<ICartesianAxis> PrivateBytesYAxes => _privateBytesChart.YAxes;

        public ObservableCollection<ISeries> WorkingSetChartSeries => _workingSetChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> WorkingSetXAxes => _workingSetChart.XAxes;
        public ObservableCollection<ICartesianAxis> WorkingSetYAxes => _workingSetChart.YAxes;

        private ProcessInformationDTO _model = default!;

        public ActionsViewModel(ProcessMetricsHostedService service)
        {
            _cpuChart = new MemoryUsageChart(
               _cpuUsageValues,
               new MemorySize("Cpu", 1),
               SKColors.MistyRose,
               40);

            _privateBytesChart = new MemoryUsageChart(
                _privateBytesValues,
                new MemorySize("Private Bytes (MB)", 1048576),
                SKColors.Red,
                40);

            _workingSetChart = new MemoryUsageChart(
                _workingSetValues,
                new MemorySize("WorkingSet (MB)", 1048576),
                SKColors.Yellow,
               40);

            _service = service;
            _dispatcher = DispatcherQueue.GetForCurrentThread();
            _timer = new Timer(UpdateMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
           

            KillCommand = new RelayCommand(OnKillButtonClicked);
        }

        private void UpdateMetrics(object? state)
        {
            if (_dispatcher is not null)
            {
                _dispatcher.TryEnqueue(() =>
                { 

                    if (_service.Processes.FirstOrDefault(p => p.PID == TargetProcessId) is not null)
                    {
                        _model = _service.Processes.FirstOrDefault(p => p.PID == TargetProcessId)!;

                        _cpuUsageValues.Add(_model.CpuUsage);
                        _privateBytesValues.Add(_model.PrivateBytes / 1048576);
                        _workingSetValues.Add(_model.WorkingSet / 1048576);

                        ProcessAddress = _model.Name;
                        IsLoading = false;
                    }
                    else
                    {
                        _cpuUsageValues.Add(0);
                        _privateBytesValues.Add(0);
                        _workingSetValues.Add(0);

                        IsLoading = true;
                        ProcessAddress = "NULL";
                    }
                    if (_cpuUsageValues.Count > 100) _cpuUsageValues.RemoveAt(0);
                });
            }
        }

        public void OnKillButtonClicked()
        {
            if (TargetProcessId > 0)
            {
                TargetProcessId = -1; 
            }
        }
    }
}
