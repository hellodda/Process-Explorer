using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Process_Explorer.GUI.Models;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Process_Explorer.GUI.Helpers
{
    public class MemoryUsageChart : ObservableObject
    {
        private readonly SKColor _strokeColor;
        private readonly byte _fillAlpha;

        private MemorySize _oldSelectedSize;

        public ObservableCollection<ISeries> ChartSeries { get; private set; }
        public ObservableCollection<ICartesianAxis> XAxes { get; private set; }
        public ObservableCollection<ICartesianAxis> YAxes { get; private set; }
        public ObservableCollection<MemorySize> MemorySizes { get; } = InitializeSizes();

        private ObservableCollection<double> _values;

        private MemorySize _selectedMemorySize;
        public MemorySize SelectedMemorySize
        {
            get => _selectedMemorySize;
            set
            {
                if (SetProperty(ref _selectedMemorySize, value))
                {
                    if (_selectedMemorySize.Value > _oldSelectedSize.Value)
                    {
                        for (int i = 0; i < _values.Count; ++i)
                        {
                            _values[i] = 0;
                        }
                        InitializeAxes();
                        InitializeSeries();
                    }
                
                    _oldSelectedSize = _selectedMemorySize;
                    OnPropertyChanged(nameof(ChartSeries));
                    OnPropertyChanged(nameof(XAxes));
                    OnPropertyChanged(nameof(YAxes));
                }
            }
        }

        public MemoryUsageChart(
            ObservableCollection<double> values,
            MemorySize initialSize,
            SKColor strokeColor,
            byte fillAlpha)
        {
            _values = values;
            _strokeColor = strokeColor;
            _fillAlpha = fillAlpha;

            _oldSelectedSize = initialSize;
            _selectedMemorySize = initialSize;

            InitializeAxes();
            InitializeSeries();
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
            => new ObservableCollection<MemorySize>
            {
                new MemorySize("B", 1),
                new MemorySize("KB", 1024),
                new MemorySize("MB", 1048576),
                new MemorySize("GB", 1073741824)
            };
    }
}
