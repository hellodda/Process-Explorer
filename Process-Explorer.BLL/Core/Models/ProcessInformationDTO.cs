namespace Process_Explorer.BLL.Core.Models
{
    public class ProcessInformationDTO
    {
        public uint PID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public uint WorkingSet { get; set; }
        public uint PrivateUsage { get; set; }
    }
}
