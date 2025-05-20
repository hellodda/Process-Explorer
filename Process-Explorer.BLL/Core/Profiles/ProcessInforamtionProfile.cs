using AutoMapper;
using Native;
using Process_Explorer.BLL.Core.Models;

namespace Process_Explorer.BLL.Core.Profiles
{
    public class ProcessInforamtionProfile : Profile
    {
        public ProcessInforamtionProfile()
        {
            CreateMap<ProcessInformation, ProcessInformationDTO>();
            CreateMap<ProcessInformationDTO, ProcessInformation>();
        }
    }
}
