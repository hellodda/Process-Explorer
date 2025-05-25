using Native;
using Process_Explorer.BLL.Models;

namespace Process_Explorer.BLL.Services
{
    public interface IProcessService
    {
        public Task<IEnumerable<Process?>> GetActiveProcessesAsync();
        public Task<Process?> GetProcessByIdAsync(int pid);
        public Task<IEnumerable<ProcessInformationDTO?>?> GetProcessesInformation();
        public Task<IEnumerable<ProcessInformationDTO?>?> GetProcessesInformation(IEnumerable<Process> processes);
        public Task<ProcessInformationDTO?> GetProcessInformationByIdAsync(int pid);
        public Task KillProcess(int pid);
    }
}
