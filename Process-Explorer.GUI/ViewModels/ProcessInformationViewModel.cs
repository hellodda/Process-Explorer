using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Process_Explorer.BLL.Models;
using Process_Explorer.GUI.Helpers;

namespace Process_Explorer.GUI.ViewModels;

public class ProcessInformationViewModel : ObservableRecipient
{
    private readonly ProcessInformationDTO _dto;

    public ProcessInformationViewModel(ProcessInformationDTO dto)
    {
        _dto = dto;
        _ = LoadIconAsync();
    }

    public uint PID => _dto.PID;
    public string Name => _dto.Name;
    public string Company => _dto.Company;
    public string Description => _dto.Description;
    public string FilePath => _dto.FilePath;

    private Microsoft.UI.Xaml.Media.Imaging.BitmapImage _iconSource = default!;
    public Microsoft.UI.Xaml.Media.Imaging.BitmapImage IconSource
    {
        get => _iconSource;
        private set
        {
            _iconSource = value;
            OnPropertyChanged();
        }
    }

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

    public double CpuUsage
    {
        get => _dto.CpuUsage;
        set
        {
            if (_dto.CpuUsage != value)
            {
                _dto.CpuUsage = value;
                OnPropertyChanged();
            }
        }
    }

    private async Task LoadIconAsync()
    {
        var icon = await IconHelper.GetIconAsync(_dto.FilePath);
        if (icon is not null)
        {
            IconSource = icon;
        }
        else
        {
            IconSource = await IconHelper.GetDefaultIcon();
        }
            
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
