using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Process_Explorer.GUI.Contracts.Services;
using Process_Explorer.GUI.ViewModels;
using Windows.System;

namespace Process_Explorer.GUI.Views;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel{ get; }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = ContentFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        App.Window.ExtendsContentIntoTitleBar = true;
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }
}
