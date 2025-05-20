using AutoMapper;
using Microsoft.Extensions.Logging;
using Process_Explorer.BLL.Core.Models;

namespace Process_Explorer.BLL.Core.Services
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
            var processes = Native.ProcessManager.GetActiveProcesses().ToList();
            var processInfoList = new List<ProcessInformationDTO>();

            foreach (var process in processes)
            {
                var info = process?.GetProcessInformation()!;
                processInfoList.Add(_mapper.Map<ProcessInformationDTO>(info));
            }
            return Task.FromResult<IEnumerable<ProcessInformationDTO>>(processInfoList);
        }
    }
}
