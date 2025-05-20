using AutoMapper;
using Process_Explorer.BLL.Core.Models;

namespace Process_Explorer.BLL
{
    public class Tester
    {
       private readonly IMapper _mapper;

        public Tester(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<ProcessInformationDTO> GetInfo()
        {
            var list = Native.ProcessManager.GetActiveProcesses().ToList();
            var pilist = new List<ProcessInformationDTO>();

            foreach (var process in list) {

                var info = process?.GetProcessInformation()!;
           
                pilist.Add(_mapper.Map<ProcessInformationDTO>(info));
            }
            return pilist;
        }
    }
}
