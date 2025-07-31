using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.BLL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Process_Explorer.GUI.ViewModels
{

    public class ProcessListViewModel : ObservableRecipient, IDisposable
    {
        private readonly ProcessMetricsHostedService _service;
        private readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();

        public ObservableCollection<ProcessInformationViewModel> ProcessList { get; } = new();

        public ProcessListViewModel(ProcessMetricsHostedService service)
        {
            _service = service;
            _service.ProcessesUpdated += OnProcessesUpdated;
        }

        private async void OnProcessesUpdated(IReadOnlyList<ProcessInformationDTO> processes)
        {
            await _dispatcher.EnqueueAsync(() =>
            {
                var existing = ProcessList.ToDictionary(vm => vm.PID);

                foreach (var dto in processes.Where(p => p.PID != 0))
                {
                    if (existing.TryGetValue(dto.PID, out var vm))
                    {
                        vm.WorkingSet = dto.WorkingSet;
                        vm.PrivateBytes = dto.PrivateBytes;
                        vm.CpuUsage = Math.Truncate(dto.CpuUsage * 10) / 10;
                    }
                    else
                    {
                        ProcessList.Add(new ProcessInformationViewModel(dto));
                    }
                }

                for (int i = ProcessList.Count - 1; i >= 0; --i)
                {
                    if (!processes.Any(p => p.PID == ProcessList[i].PID))
                    {
                        ProcessList.RemoveAt(i);
                    }
                }
            });
        }

        public void Dispose()
        {
            _service.ProcessesUpdated -= OnProcessesUpdated;
        }
    }
}