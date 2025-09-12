using CommunityToolkit.WinUI;
using Process_Explorer.GUI.ViewModels;
using Process_Explorer.GUI.Views.Pages;

namespace Process_Explorer.GUI.Views;

public sealed partial class ShellPage : Microsoft.UI.Xaml.Controls.Page
{
    public ShellViewModel ViewModel{ get; }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = ContentFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        App.Window.ExtendsContentIntoTitleBar = true;
        App.Window.Title = "AppDisplayName".GetLocalized();
        App.Window.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();

        TableFrame.Navigate(typeof(TablePage));
    }
}
