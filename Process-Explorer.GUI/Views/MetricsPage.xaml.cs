using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI.Views
{
    public sealed partial class MetricsPage : Page
    {
        public MetricsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = e.Parameter as MetricsViewModel;
        }
    }
}
