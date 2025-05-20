using Process_Explorer.BLL.Core.Models;

namespace Process_Explorer.BLL.Core.Services
{
    public interface IProcessService
    {
        public Task<IEnumerable<ProcessInformationDTO>> GetActiveProcessesAsync();
    }
}
