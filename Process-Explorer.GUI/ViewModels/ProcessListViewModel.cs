using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Process_Explorer.BLL.HostedServices;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Process_Explorer.GUI.ViewModels
{
    public class ProcessListViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ProcessMetricsHostedService _service;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly Timer _timer;

        public ObservableCollection<ProcessInformationViewModel> ProcessList { get; } = new();

        public ProcessListViewModel(ProcessMetricsHostedService service)
        {
            _service = service;
            _timer = new Timer(OnTimerTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            LoadProcesses();
        }

        private async void OnTimerTick(object? state) => await UpdateProcessesAsync();

        public void LoadProcesses()
        {
            var processes = _service.processes;
            _dispatcherQueue.TryEnqueue(() =>
            {
                ProcessList.Clear();
                foreach (var p in processes)
                {
                    if (p.PID == 0) continue;
                    ProcessList.Add(new ProcessInformationViewModel(p));
                }
            });
        }

        public async Task UpdateProcessesAsync()
        {
            var newProcesses = _service.processes;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                var existingByPid = ProcessList.ToDictionary(vm => vm.PID);

                foreach (var dto in newProcesses)
                {
                    if (dto.PID == 0) continue;
                    if (existingByPid.TryGetValue(dto.PID, out var vm))
                    {
                        vm.WorkingSet = dto.WorkingSet;
                        vm.PrivateBytes = dto.PrivateBytes;
                    }
                    else
                    {
                        ProcessList.Add(new ProcessInformationViewModel(dto));
                    }
                }

                for (int i = ProcessList.Count - 1; i >= 0; i--)
                {
                    if (!newProcesses.Any(p => p.PID == ProcessList[i].PID))
                        ProcessList.RemoveAt(i);
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
