using AutoMapper;
using Microsoft.Extensions.Logging;
using Process_Explorer.BLL.Models;

namespace Process_Explorer.BLL.Services
{
    public class ProcessService : IProcessService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ProcessService(IMapper mapper, ILogger<ProcessService> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public Task<IEnumerable<ProcessInformationDTO>> GetActiveProcessesAsync()
        {
            _logger.LogDebug("GetActiveProcessesAsync called");
            try
            {
                var processes = Native.ProcessManager.GetActiveProcesses().ToList();
                var processInfoList = new List<ProcessInformationDTO>();

                foreach (var process in processes)
                {
                    var info = process?.GetProcessInformation()!;
                    if (info.PID is 0)
                    {
                        _logger.LogWarning($"Process with pid {info.PID} has no information");
                        continue;
                    }
                    _logger.LogDebug($"information about process with pid {info.PID} received");
                    processInfoList.Add(_mapper.Map<ProcessInformationDTO>(info));
                }
                return Task.FromResult<IEnumerable<ProcessInformationDTO>>(processInfoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active processes");
                throw;
            }
        }

        public Task<ProcessInformationDTO?> GetProcessByIdAsync(int pid)
        {
            _logger.LogDebug($"GetProcessByIdAsync called for PID: {pid}");
            try
            {
                var processes = Native.ProcessManager.GetActiveProcesses().ToList();
                var process = processes.FirstOrDefault(p => p?.GetProcessInformation().PID == pid);
                if (process is null)
                {
                    _logger.LogError($"not found process for PID: {pid}");
                }
                return Task.FromResult<ProcessInformationDTO?>(_mapper.Map<ProcessInformationDTO>(process.GetProcessInformation()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active processes");
                throw;
            }
        }
    }
}
