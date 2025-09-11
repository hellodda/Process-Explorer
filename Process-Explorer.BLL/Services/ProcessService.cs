using AutoMapper;
using Microsoft.Extensions.Logging;
using Native;
using Process_Explorer.BLL.Models;

namespace Process_Explorer.BLL.Services
{
    public class ProcessService : IProcessService
    {
        private readonly IMapper _mapper;
        private readonly IProcessManager _manager;
        private readonly IProcessInformationProvider _provider;
        private readonly ILogger _logger;

        public ProcessService(IMapper mapper, IProcessManager manager, IProcessInformationProvider provider, ILogger<ProcessService> logger)
        {
            _mapper = mapper;
            _manager = manager;
            _provider = provider;
            _logger = logger;
        }

        public async Task<IEnumerable<Process?>> GetActiveProcessesAsync()
        {
            _logger.LogDebug("GetActiveProcessesAsync called");
            try
            {
                var processes = (await _manager.GetActiveProcessesAsync()).ToList();

                return processes!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active processes");
                throw;
            }
        }

        public async Task<Process?> GetProcessByIdAsync(int pid)
        {
            _logger.LogDebug($"GetProcessByIdAsync called for PID: {pid}");
            try
            {
                var processes = (await _manager.GetActiveProcessesAsync()).ToList();;
                var process = processes.FirstOrDefault(p => _provider.GetProcessInformation(p).PID == pid);
                if (process is null)
                {
                    _logger.LogError($"not found process for PID: {pid}");
                }
                return process!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active processes");
                throw;
            }
        }

        public async Task<IEnumerable<ProcessInformationDTO?>?> GetProcessesInformationAsync()
        {
            _logger.LogDebug("GetProcessesInformation called");

            return await GetProcessesInformationAsync(await _manager.GetActiveProcessesAsync());
        }

        public Task<IEnumerable<ProcessInformationDTO?>?> GetProcessesInformationAsync(IEnumerable<Process> processes)
        {
            _logger.LogDebug("GetProcessesInformation(procesess) called");
            try
            {
                var processInfoList = new List<ProcessInformationDTO>();

                foreach (var process in processes)
                {
                    var info = _provider.GetProcessInformation(process)!;
                    if (info is null)
                    {
                        _logger.LogWarning($"Process has no information");
                        continue;
                    }
                    if (info.PID is 0)
                    {
                        _logger.LogWarning($"Process with pid {info.PID} has no information");
                        continue;
                    }
                    _logger.LogDebug($"information about process with pid {info.PID} received");
                    processInfoList.Add(_mapper.Map<ProcessInformationDTO>(info));
                }
                return Task.FromResult<IEnumerable<ProcessInformationDTO>>(processInfoList)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active processes");
                throw;
            }
        }

        public async Task<ProcessInformationDTO?> GetProcessInformationByIdAsync(int pid)
        {
            _logger.LogDebug($"GetProcessInformationByIdAsync called for PID: {pid}");
            try
            {
                var processes = (await _manager.GetActiveProcessesAsync()).ToList();
                var process = processes.FirstOrDefault(p => _provider.GetProcessInformation(p).PID == pid);
                if (process is null)
                {
                    _logger.LogError($"not found process for PID: {pid}");
                }
                return _mapper.Map<ProcessInformationDTO>(_provider.GetProcessInformation(process))!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active processes");
                throw;
            }
        }

        public async Task KillProcess(int pid)
        {
            _logger.LogDebug($"KillProcess called for PID: {pid}");
            try
            {
                var process = await GetProcessByIdAsync(pid);
                if (process is null)
                {
                    _logger.LogError($"Process with PID {pid} not found.");
                    return;
                }
                process.Terminate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while killing process");
                throw;
            }
        }
    }
}
