using AutoMapper;
using Native;
using Process_Explorer.BLL.Models;

namespace Process_Explorer.BLL.Profiles
{
    public class ProcessInforamtionProfile : Profile
    {
        public ProcessInforamtionProfile()
        {
            CreateMap<ProcessInformation, ProcessInformationDTO>();
            CreateMap<ProcessInformationDTO, ProcessInformation>();

            CreateMap<ProcessInformationEx, ProcessInformationDTO>();
            CreateMap<ProcessInformationDTO, ProcessInformationEx>();
        }
    }
}
