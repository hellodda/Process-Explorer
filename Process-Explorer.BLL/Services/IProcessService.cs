using Native;
using Process_Explorer.BLL.Models;

namespace Process_Explorer.BLL.Services
{
    public interface IProcessService
    {
        public Task<IEnumerable<Process?>> GetActiveProcessesAsync();
        public Task<Process?> GetProcessByIdAsync(int pid);
        public Task<IEnumerable<ProcessInformationDTO?>?> GetProcessesInformationAsync();
        public Task<IEnumerable<ProcessInformationDTO?>?> GetProcessesInformationAsync(IEnumerable<ProcessEx> processes);

        public Task<ProcessInformationDTO?> GetProcessInformationByIdAsync(int pid);
        public Task KillProcess(int pid);
    }
}
