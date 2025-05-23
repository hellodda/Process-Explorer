using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using Process_Explorer.BLL.Services;
using Process_Explorer.GUI.Models;

namespace Process_Explorer.GUI.ViewModels;

public class MetricsViewModel : INotifyPropertyChanged
{
    private readonly IProcessService _service;

    private readonly ObservableCollection<double> _privateBytesValues = new();
    private readonly ObservableCollection<double> _workingSetValues = new();

    public ObservableCollection<ISeries> PrivateBytesChartSeries { get; private set; }
    public ObservableCollection<ICartesianAxis> PrivateBytesXAxes { get; private set; }
    public ObservableCollection<ICartesianAxis> PrivateBytesYAxes { get; private set; }

    public ObservableCollection<ISeries> WorkingSetChartSeries { get; private set; }
    public ObservableCollection<ICartesianAxis> WorkingSetXAxes { get; private set; }
    public ObservableCollection<ICartesianAxis> WorkingSetYAxes { get; private set; }

    public ObservableCollection<MemorySize> MemoryVarsWorkingSet { get; } = InitializeSizes();
    public ObservableCollection<MemorySize> MemoryVarsPrivateBytes { get; } = InitializeSizes();

    private MemorySize _selectedMemVarWorkingSet;
    public MemorySize SelectedMemVarWorkingSet
    {
        get => _selectedMemVarWorkingSet;
        set
        {
            if (_selectedMemVarWorkingSet != value)
            {
                _selectedMemVarWorkingSet = value;
                OnPropertyChanged();
                ReInitialize();
            }
        }
    }

    private MemorySize _selectedMemVarPrivateBytes;
    public MemorySize SelectedMemVarPrivateBytes
    {
        get => _selectedMemVarPrivateBytes;
        set
        {
            if (_selectedMemVarPrivateBytes != value)
            {
                _selectedMemVarPrivateBytes = value;
                OnPropertyChanged();
                ReInitialize();
            }
        }
    }

    public MetricsViewModel(IProcessService service)
    {
        _service = service;
        _selectedMemVarWorkingSet = MemoryVarsWorkingSet.Last(); 
        _selectedMemVarPrivateBytes = MemoryVarsPrivateBytes.Last();
      
        InitializeSeriesAndAxes();

        _ = UpdateLoopAsync();
    }

    private void InitializeSeriesAndAxes()
    {
        _privateBytesValues.Clear();
        _workingSetValues.Clear();

        PrivateBytesChartSeries = new ObservableCollection<ISeries>
        {
            CreateSeries(_privateBytesValues, SKColors.Red, 40)
        };
        PrivateBytesXAxes = new() { new Axis() };
        PrivateBytesYAxes = new() { new Axis { MinLimit = 0 } };

        WorkingSetChartSeries = new ObservableCollection<ISeries>
        {
            CreateSeries(_workingSetValues, SKColors.Yellow, 40)
        };
        WorkingSetXAxes = new() { new Axis() };
        WorkingSetYAxes = new() { new Axis { MinLimit = 0 } };

        OnPropertyChanged(nameof(PrivateBytesChartSeries));
        OnPropertyChanged(nameof(PrivateBytesXAxes));
        OnPropertyChanged(nameof(PrivateBytesYAxes));
        OnPropertyChanged(nameof(WorkingSetChartSeries));
        OnPropertyChanged(nameof(WorkingSetXAxes));
        OnPropertyChanged(nameof(WorkingSetYAxes));
    }

    private LineSeries<double> CreateSeries(
        ObservableCollection<double> values,
        SKColor color,
        byte alpha)
    {
        return new LineSeries<double>
        {
            Values = values,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, alpha)),
            GeometrySize = 0
        };
    }

    static private ObservableCollection<MemorySize> InitializeSizes()
        => new ObservableCollection<MemorySize>
        {
            new MemorySize("B", 1),
            new MemorySize("KB", 1024),
            new MemorySize("MB", 1048576),
            new MemorySize("GB", 1073741824)
        };

    private void ReInitialize()
    {
        InitializeSeriesAndAxes();
    }

    private async Task UpdateLoopAsync()
    {
        while (true)
        {
            var processes = (await _service.GetActiveProcessesAsync()).ToList();
            if (processes.Any())
            {
                var sumPrivate = processes.Sum(p => p.PrivateBytes) / SelectedMemVarPrivateBytes.Value;
                var sumWorking = processes.Sum(p => p.WorkingSet) / SelectedMemVarWorkingSet.Value;

                _privateBytesValues.Add(sumPrivate);
                _workingSetValues.Add(sumWorking);

                if (_privateBytesValues.Count > 50) _privateBytesValues.RemoveAt(0);
                if (_workingSetValues.Count > 50) _workingSetValues.RemoveAt(0);
            }

            await Task.Delay(1000);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}