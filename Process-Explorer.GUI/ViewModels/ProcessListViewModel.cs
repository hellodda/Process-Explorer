using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Process_Explorer.BLL.Models;
using Process_Explorer.BLL.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Process_Explorer.GUI.ViewModels
{
    public class ProcessListViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService _service;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private Timer _timer;

        public ObservableCollection<ProcessInformationDTO> ProcessList { get; set; } = new();

        public ProcessListViewModel(IProcessService service)
        {
            _service = service;
            _timer = new Timer(async _ => await UpdateProcesses(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            LoadProcesses();
        }

        public async Task LoadProcesses()
        {
            var processes = await _service.GetActiveProcessesAsync();

            ProcessList.Clear();
            foreach (var process in processes)
            {
                if (process.PID is 0) continue;
                ProcessList.Add(process);
            }
        }

        public async Task UpdateProcesses()
        {
            var newProcesses = (await _service.GetActiveProcessesAsync()).ToList();

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                var existingByPid = ProcessList.ToDictionary(p => p.PID);

                foreach (var newProcess in newProcesses)
                {
                    if (existingByPid.TryGetValue(newProcess.PID, out var existing))
                    {
                        existing.WorkingSet = newProcess.WorkingSet;
                    }
                    else
                    {
                        ProcessList.Add(newProcess);
                    }
                }
                for (int i = ProcessList.Count - 1; i >= 0; i--)
                {
                    var existing = ProcessList[i];
                    if (!newProcesses.Any(p => p.PID == existing.PID))
                    {
                        ProcessList.RemoveAt(i);
                    }
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessList)));
            });
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
