﻿using CommunityToolkit.Mvvm.ComponentModel;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.GUI.Helpers;
using Process_Explorer.GUI.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Process_Explorer.GUI.ViewModels
{
    public partial class MetricsViewModel : ObservableObject, IDisposable
    {
        private readonly ProcessMetricsHostedService _service;
        private readonly Timer _timer;

        public ObservableCollection<MemorySize> MemorySizes { get; } = MemoryUsageChart.InitializeSizes();

        [ObservableProperty]
        private MemorySize _selectedMemVarPrivateBytes;

        [ObservableProperty]
        private MemorySize _selectedMemVarWorkingSet;

        public MemoryUsageChart PrivateChart;
        public MemoryUsageChart WorkingChart;
        public MemoryUsageChart CpuChart;

        public MetricsViewModel(ProcessMetricsHostedService service)
        {
            _service = service;
            _selectedMemVarPrivateBytes = MemorySizes.Last();
            _selectedMemVarWorkingSet = MemorySizes.Last();

            PrivateChart = new MemoryUsageChart(
                _selectedMemVarPrivateBytes,
                SKColors.Red,
                40);

            PrivateChart.Limit = 100;

            WorkingChart = new MemoryUsageChart(
                _selectedMemVarWorkingSet,
                SKColors.Yellow,
                40);

            WorkingChart.Limit = 100;

            CpuChart = new MemoryUsageChart(
                MemorySize.Cpu,
                SKColors.MistyRose,
                40);

            CpuChart.Limit = 100;

            _timer = new Timer(UpdateMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateMetrics(object? state)
        {
            var processes = _service.Processes;

            if (!processes.Any()) return;

            var sumP = processes.Sum(p => p.PrivateBytes);
            var sumW = processes.Sum(p => p.WorkingSet);
            var sumC = processes.Sum(p => p.CpuUsage);

            PrivateChart.AddValue(sumP);
            WorkingChart.AddValue(sumW);
            CpuChart.AddValue(sumC);
        }

        partial void OnSelectedMemVarPrivateBytesChanged(MemorySize oldVal, MemorySize newVal)
        {
            PrivateChart.SelectedMemorySize = newVal;
        }

        partial void OnSelectedMemVarWorkingSetChanged(MemorySize oldVal, MemorySize newVal)
        {
            WorkingChart.SelectedMemorySize = newVal;
        }

        public void Dispose() => _timer?.Dispose();
    }
}