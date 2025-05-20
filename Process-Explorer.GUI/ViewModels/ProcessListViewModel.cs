using Process_Explorer.BLL.Core.Models;
using Process_Explorer.BLL.Core.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Process_Explorer.GUI.ViewModels
{
    public class ProcessListViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService _service;

        public ObservableCollection<ProcessInformationDTO> ProcessList { get; set; } = new();

        public ProcessListViewModel(IProcessService service)
        {
            _service = service;
            LoadProcesses();
        }

        private async Task LoadProcesses()
        {
            var processes = await _service.GetActiveProcessesAsync();

            ProcessList.Clear();
            foreach (var process in processes)
            {
                if (process.PID is 0) continue;
                ProcessList.Add(process);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
