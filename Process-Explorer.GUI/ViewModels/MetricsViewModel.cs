using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.ComponentModel;
using Process_Explorer.BLL.Services;

namespace Process_Explorer.GUI.ViewModels
{
    public class MetricsViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService _service;


        public ObservableCollection<ISeries> ChartSeries { get; set; }
        public ObservableCollection<ICartesianAxis> XAxes { get; set; }
        public ObservableCollection<ICartesianAxis> YAxes { get; set; }

        public MetricsViewModel(IProcessService service)
        {
            _service = service;
            ChartSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = new double[] { 10, 20, 30, 40, 35 },
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0,
                    GeometryFill = null,
                    GeometryStroke = null
                }
            };

            XAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis { }
            };

            YAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis { MinLimit = 0, MaxLimit = 100 }
            };

        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
