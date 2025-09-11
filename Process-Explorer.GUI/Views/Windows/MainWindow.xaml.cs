using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

namespace Process_Explorer.GUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        //AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        ExtendsContentIntoTitleBar = true;
        Content = null;
        Title = "AppDisplayName".GetLocalized();
    }
}
