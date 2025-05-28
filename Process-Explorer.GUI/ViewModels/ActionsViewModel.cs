using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.BLL.Models;
using Process_Explorer.GUI.Helpers;
using Process_Explorer.GUI.Models;
using SkiaSharp;
using System;
using System.Linq;
using System.Threading;

namespace Process_Explorer.GUI.ViewModels
{
    public partial class ActionsViewModel : ObservableObject
    {
        private readonly ProcessMetricsHostedService _service = default!;
        private readonly Timer _timer = default!;

        private readonly DispatcherQueue _dispatcher = default!;

        public MemoryUsageChart CpuChart;
        public MemoryUsageChart PrivateChart;
        public MemoryUsageChart WorkingChart;

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

                    CpuChart.Reinitialize();
                    PrivateChart.Reinitialize();
                    WorkingChart.Reinitialize();
                }
            }
        }

        public IRelayCommand KillCommand { get; } 

        private ProcessInformationDTO _model = default!;

        public ActionsViewModel(ProcessMetricsHostedService service)
        {
            CpuChart = new MemoryUsageChart(
               MemorySize.Cpu,
               SKColors.MistyRose,
               40);

            CpuChart.Limit = 100;

            PrivateChart = new MemoryUsageChart(
                MemorySize.MegaByte,
                SKColors.Red,
                40);

            PrivateChart.Limit = 100;

            WorkingChart = new MemoryUsageChart(
                MemorySize.MegaByte,
                SKColors.Yellow,
               40);

            WorkingChart.Limit = 100;

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

                        CpuChart.AddValue(_model.CpuUsage);
                        PrivateChart.AddValue(_model.PrivateBytes);
                        WorkingChart.AddValue(_model.WorkingSet);

                        ProcessAddress = _model.Name;
                        IsLoading = false;
                    }
                    else
                    {
                        IsLoading = true;
                        ProcessAddress = "NULL";
                    }
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
