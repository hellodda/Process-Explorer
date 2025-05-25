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

namespace Process_Explorer.GUI.ViewModels
{
    ///------------------------------------------------
    /// 
    /// Skoro...
    /// 
    ///------------------------------------------------
    public class ActionsViewModel : ObservableObject
    {
        private readonly ProcessMetricsHostedService _service;
        private readonly Timer _timer;
        private readonly ObservableCollection<double> _privateValues = new();
        private readonly DispatcherQueue _dispatcher;

        private MemoryUsageChart _cpuChart;

        private string _processAddress;
        public string ProcessAddress
        {
            get => _processAddress;
            set
            {
                if (_processAddress != value)
                {
                    _processAddress = value;
                    OnPropertyChanged(nameof(ProcessAddress));
                }
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        private int _targetProcessId;
        public int TargetProcessId
        {
            get => _targetProcessId;
            set
            {
                if (_targetProcessId != value)
                {
                    _targetProcessId = value;
                    _cpuChart.Reinitialize();
                    ProcessAddress = "Processing...";
                    OnPropertyChanged(nameof(TargetProcessId));
                    OnPropertyChanged(nameof(CpuChartSeries));
                }
            }
        }

        private double _targetProcessCPUUsage;
        public double TargetProcessCPUUsage
        {
            get => _targetProcessCPUUsage;
            set
            {
                if (_targetProcessCPUUsage != value)
                {
                    _targetProcessCPUUsage = value;
                    OnPropertyChanged(nameof(TargetProcessCPUUsage));
                }
            }
        }

        public IRelayCommand KillCommand { get; }

        public ObservableCollection<ISeries> CpuChartSeries => _cpuChart.ChartSeries;
        public ObservableCollection<ICartesianAxis> CpuXAxes => _cpuChart.XAxes;
        public ObservableCollection<ICartesianAxis> CpuYAxes => _cpuChart.YAxes;

        public ActionsViewModel(ProcessMetricsHostedService service)
        {
            _cpuChart = new MemoryUsageChart(
               _privateValues,
               new MemorySize("Cpu", 1),
               SKColors.MistyRose,
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
                    if (TargetProcessCPUUsage > 0) IsLoading = false;
                    if (TargetProcessCPUUsage <= 0) IsLoading = true;

                    _privateValues.Add(TargetProcessCPUUsage);
                    if (_privateValues.Count > 100) _privateValues.RemoveAt(0);
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
