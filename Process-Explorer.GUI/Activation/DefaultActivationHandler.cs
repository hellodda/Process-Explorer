using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Process_Explorer.GUI.Contracts.Services;
using Process_Explorer.GUI.ViewModels;

namespace Process_Explorer.GUI.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return _navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        _navigationService.NavigateTo(typeof(ShellViewModel).FullName!, args.Arguments);

        await Task.CompletedTask;
    }
}
