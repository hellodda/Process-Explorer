using Process_Explorer.BLL.Models;

namespace Process_Explorer.BLL.Services
{
    public interface IProcessService
    {
        public Task<IEnumerable<ProcessInformationDTO>> GetActiveProcessesAsync();
        public Task<ProcessInformationDTO?> GetProcessByIdAsync(int pid);
    }
}
