using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.ComponentModel;
using Process_Explorer.BLL.Services;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;



namespace Process_Explorer.GUI.ViewModels
{
    public class MetricsViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService _service;


        public ObservableCollection<ISeries> PrivateBytesChartSeries { get; set; } = default!;
        public ObservableCollection<ICartesianAxis> PrivateBytesXAxes { get; set; } = default!;
        public ObservableCollection<ICartesianAxis> PrivateBytesYAxes { get; set; } = default!;

        public ObservableCollection<ISeries> WorkingSetChartSeries { get; set; } = default!;
        public ObservableCollection<ICartesianAxis> WorkingSetXAxes { get; set; } = default!;
        public ObservableCollection<ICartesianAxis> WorkingSetYAxes { get; set; } = default!;

        public MetricsViewModel(IProcessService service)
        {
            _service = service;
            LoadData().Wait(); 
        }

        private async Task LoadData()
        {
            var processes = (await _service.GetActiveProcessesAsync()).ToList();

            List<double> privateUsages = new List<double>();
            List<double> workingSets = new List<double>();

            foreach (var process in processes)
            {
                privateUsages.Add(process.PrivateBytes / 1048576);
                workingSets.Add(process.WorkingSet / 1048576);
            }

            PrivateBytesChartSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = privateUsages,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0,
                    GeometryFill = null,
                    GeometryStroke = null
                }
            };

            PrivateBytesXAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis { }
            };

            PrivateBytesYAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis { }
            };


            WorkingSetChartSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = workingSets,
                    Stroke = new SolidColorPaint(SKColors.Yellow) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0,
                    GeometryFill = null,
                    GeometryStroke = null
                }
            };

            WorkingSetXAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis { }
            };

            WorkingSetYAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis { }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
