using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI.Views;

public sealed partial class MetricsPage : Page
{
    private MetricsViewModel viewModel = default!;

    public MetricsPage()
    {
        viewModel = App.GetService<MetricsViewModel>();
        this.InitializeComponent();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;
    }
}
