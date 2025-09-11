using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Process_Explorer.GUI.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;

namespace Process_Explorer.GUI.Helpers;

public class MemoryUsageChart : ObservableObject
{
    private readonly SKColor _strokeColor;
    private readonly byte _fillAlpha;

    private MemorySize _oldSelectedSize = default!;

    public ObservableCollection<ISeries> ChartSeries { get; private set; } = default!;
    public ObservableCollection<ICartesianAxis> XAxes { get; private set; } = default!;
    public ObservableCollection<ICartesianAxis> YAxes { get; private set; } = default!;

    public ObservableCollection<MemorySize> MemorySizes { get; } = InitializeSizes();

    private ObservableCollection<double> _values { get; set; } = new();
    public int Limit { get; set; } = 100;

    private MemorySize _selectedMemorySize;
    public MemorySize SelectedMemorySize
    {
        get => _selectedMemorySize;
        set
        {
            if (SetProperty(ref _selectedMemorySize, value))
            {
                Reinitialize();
            
                _oldSelectedSize = _selectedMemorySize;
                OnPropertyChanged(nameof(ChartSeries));
                OnPropertyChanged(nameof(XAxes));
                OnPropertyChanged(nameof(YAxes));
            }
        }
    }

    public MemoryUsageChart(
        MemorySize initialSize,
        SKColor strokeColor,
        byte fillAlpha)
    {
        _strokeColor = strokeColor;
        _fillAlpha = fillAlpha;

        _oldSelectedSize = initialSize;
        _selectedMemorySize = initialSize;

        InitializeAxes();
        InitializeSeries();

        _values.Add(0); //initial value ;)
    }

    public void Reinitialize()
    {
        for (int i = 0; i < _values.Count; ++i)
        {
            _values[i] = 0;
        }

        InitializeAxes();
        InitializeSeries();
    }

    public void AddValue(double value, bool round = true)
    {
        var newValue = round ? Math.Round(_selectedMemorySize.Calculate(value)) : _selectedMemorySize.Calculate(value);

        _values.Add(newValue);

        if (_values.Count > Limit) _values.RemoveAt(0);
    }

    private void InitializeAxes()
    {
        XAxes = new ObservableCollection<ICartesianAxis> { new Axis() };
        YAxes = new ObservableCollection<ICartesianAxis> { new Axis { MinLimit = 0 } };
    }

    private void InitializeSeries()
    {
        ChartSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<double>
            {
                Values = _values,
                Stroke = new SolidColorPaint(_strokeColor) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(new SKColor(_strokeColor.Red, _strokeColor.Green, _strokeColor.Blue, _fillAlpha)),
                GeometrySize = 0,
                YToolTipLabelFormatter = point => $"{point.Context.DataSource} {_selectedMemorySize.Name}"
            }
        };
    }

    public static ObservableCollection<MemorySize> InitializeSizes()
        => new()
        {
            MemorySize.Byte,
            MemorySize.KiloByte,
            MemorySize.MegaByte,
            MemorySize.GigaByte,
        };
}
