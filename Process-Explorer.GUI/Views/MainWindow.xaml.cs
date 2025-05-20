using Microsoft.UI.Xaml;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI
{
    public sealed partial class MainWindow : Window
    {
        private readonly ProcessListViewModel _viewModel;
 
        public MainWindow(ProcessListViewModel viewModel)
        {
            InitializeComponent();

            //----------
            
            _viewModel = viewModel;
         
            //----------

            Setup();  
        }

        public void Setup()
        {
            ExtendsContentIntoTitleBar = true;
        }
    }
}
