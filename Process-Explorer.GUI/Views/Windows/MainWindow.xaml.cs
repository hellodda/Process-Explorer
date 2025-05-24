using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Process_Explorer.GUI.ViewModels;
using Process_Explorer.GUI.Views;
using System;

namespace Process_Explorer.GUI
{
    public sealed partial class MainWindow : Window
    {
        private readonly ProcessListViewModel _viewModel;
        private readonly IServiceProvider _provider;
  
        public MainWindow(IServiceProvider provider, ProcessListViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _provider = provider;

            Setup();  
        }

        public void Setup()
        {
            ExtendsContentIntoTitleBar = true;
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                string tag = item.Tag.ToString()!;

                switch (tag)
                {
                    case "actions":
                        ContentFrame.Navigate(typeof(ActionsPage), _provider.GetRequiredService<ActionsViewModel>());
                        break;
                    case "metrics":
                        ContentFrame.Navigate(typeof(MetricsPage), _provider.GetRequiredService<MetricsViewModel>());
                        break;
                    default:
                        break;
                }
            }
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack) ContentFrame.GoBack();
        }
    }
}
