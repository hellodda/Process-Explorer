using Microsoft.UI.Xaml.Controls;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI.Views.Pages;

public sealed partial class TablePage : Page
{
    private readonly TableViewModel _viewModel = default!;

    public TablePage()
    {
        _viewModel = App.GetService<TableViewModel>();

        InitializeComponent();
    }
}
