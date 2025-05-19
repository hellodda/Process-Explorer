using Microsoft.UI.Xaml;
using Process_Explorer.BLL;

namespace Process_Explorer.GUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Tester.GetInfo();
        }
    }
}
