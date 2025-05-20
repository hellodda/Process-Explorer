using Microsoft.UI.Xaml;
using Process_Explorer.BLL;
using Process_Explorer.BLL.Core.Models;
using Process_Explorer.GUI.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Process_Explorer.GUI
{
    public sealed partial class MainWindow : Window
    {
        private readonly ProcessListViewModel _viewModel;
        public ObservableCollection<ProcessInformationDTO> ProcessList { get; set; } = new();

        public MainWindow(ProcessListViewModel viewModel)
        {
            InitializeComponent();

            //----------
            
            _viewModel = viewModel;
            ProcessList = _viewModel.ProcessList;

            //----------

            Setup();  
        }

        public void Setup()
        {
            ExtendsContentIntoTitleBar = true;
           

        }
    }
}
