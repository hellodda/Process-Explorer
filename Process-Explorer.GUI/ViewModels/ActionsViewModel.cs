using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.BLL.Models;
using Process_Explorer.GUI.Helpers;
using Process_Explorer.GUI.Models;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Process_Explorer.GUI.ViewModels
{
    public partial class ActionsViewModel : ObservableObject
    {
        private readonly ProcessMetricsHostedService _service = default!;
        private readonly DispatcherQueue _dispatcher = default!;

        public MemoryUsageChart CpuChart = default!;
        public MemoryUsageChart PrivateChart = default!;
        public MemoryUsageChart WorkingChart = default!;

        [ObservableProperty]
        private Visibility _progressBarVisiblity;

        [ObservableProperty]
        private string _processAddress;

        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private string _text = "Enter Process Id [PID]";

        private int _targetProcessId = -1;
        public int TargetProcessId
        {
            get => _targetProcessId;
            set
            {
                if (SetProperty(ref _targetProcessId, value))
                {
                    Text = (value == 0) ? "Idle Process [Information Cannot Be Obtained]" : "Enter Process Id [PID]";

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


            KillCommand = new RelayCommand(OnKillButtonClicked);

            _service.ProcessesUpdated += OnProcessesUpdated;
        }

        private void OnProcessesUpdated(IReadOnlyList<ProcessInformationDTO> processes)
        {
            _dispatcher.TryEnqueue(() =>
            {
                var process = processes.FirstOrDefault(p => p.PID == TargetProcessId);

                if (process is not null)
                {
                    _model = process;
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

        public void OnKillButtonClicked()
        {
            if (TargetProcessId > 0)
            {
                TargetProcessId = -1;
            }
        }

        public void Dispose()
        {
            _service.ProcessesUpdated -= OnProcessesUpdated;
        }
    }
}