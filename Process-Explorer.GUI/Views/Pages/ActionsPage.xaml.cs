using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI.Views
{
    public sealed partial class ActionsPage : Page
    {
        private ActionsViewModel viewModel = default!;

        public ActionsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel = e.Parameter as ActionsViewModel;
        }
    }
}
