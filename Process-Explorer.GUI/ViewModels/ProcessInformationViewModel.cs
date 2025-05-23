using System.ComponentModel;
using System.Runtime.CompilerServices;
using Process_Explorer.BLL.Models;

namespace Process_Explorer.GUI.ViewModels
{
    public class ProcessInformationViewModel : INotifyPropertyChanged
    {
        private readonly ProcessInformationDTO _dto;

        public ProcessInformationViewModel(ProcessInformationDTO dto)
        {
            _dto = dto;
        }

        public uint PID => _dto.PID;
        public string Name => _dto.Name;
        public string Company => _dto.Company;
        public string Description => _dto.Description;

        public uint WorkingSet
        {
            get => _dto.WorkingSet;
            set
            {
                if (_dto.WorkingSet != value)
                {
                    _dto.WorkingSet = value;
                    OnPropertyChanged();
                }
            }
        }

        public uint PrivateBytes
        {
            get => _dto.PrivateBytes;
            set
            {
                if (_dto.PrivateBytes != value)
                {
                    _dto.PrivateBytes = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}