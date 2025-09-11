using Microsoft.UI.Xaml.Controls;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI.Views.Pages;

public sealed partial class TablePage : Page
{
    private TableViewModel _viewModel = default!;

    public TablePage(TableViewModel viewModel)
    {
        _viewModel = viewModel;

        InitializeComponent();
    }
}
