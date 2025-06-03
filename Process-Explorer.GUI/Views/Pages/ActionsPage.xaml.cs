using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Process_Explorer.GUI.ViewModels;
using System;
using System.Drawing;

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

        private async void _____()
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Soon :)",
                Content = "I don't have time to update often.",
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel = e.Parameter as ActionsViewModel;
        }

        //This will be fixed in the future :)
        //so don't pay attention
        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var notification = new AppNotificationBuilder()
            .AddText("ops...")
            .AddText("soon bro soon")
            .BuildNotification();

            AppNotificationManager.Default.Show(notification);

        }

        private void Button_Click_1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _____();
        }

        private void Button_Click_2(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _____();
        }
    }
}
