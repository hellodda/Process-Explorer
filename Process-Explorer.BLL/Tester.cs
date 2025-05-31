using AutoMapper;
using Process_Explorer.BLL.Models;
using System.Threading.Tasks;

namespace Process_Explorer.BLL
{
    public class Tester
    {
       private readonly IMapper _mapper;

        public Tester(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProcessInformationDTO>> GetInfo()
        {
            var list = (await Native.ProcessManager.GetActiveProcessesAsync()).ToList();
            var pilist = new List<ProcessInformationDTO>();

            foreach (var process in list) {

                var info = process?.GetProcessInformation()!;
           
                pilist.Add(_mapper.Map<ProcessInformationDTO>(info));
            }
            return pilist;
        }
    }
}
