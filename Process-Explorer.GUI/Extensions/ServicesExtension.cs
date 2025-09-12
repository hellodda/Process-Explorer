using Microsoft.Extensions.DependencyInjection;
using Process_Explorer.BLL;
using Process_Explorer.GUI.ViewModels;
using Process_Explorer.BLL.Services;
using Process_Explorer.BLL.Profiles;
using Process_Explorer.GUI.Views;
using Process_Explorer.BLL.HostedServices;
using Native;
using Process_Explorer.GUI.Views.Pages;
using Process_Explorer.GUI.Contracts.Services;
using Process_Explorer.GUI.Services;
using Process_Explorer.GUI.Activation;
using Microsoft.UI.Xaml;

namespace Process_Explorer.GUI.Extensions;

public static class ServicesExtension
{
    public static void ConfigureServices(this IServiceCollection? services)
    {
        services!.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

        // native services

        services!.AddTransient<IProcessService, ProcessService>();
        services!.AddTransient<IProcessManager, ProcessManager>();
        services!.AddTransient<IProcessInformationProvider, ProcessInformationProvider>();
        services!.AddTransient<ICpuUsageCalculator, CpuUsageCalculator>();
        
        // navigation helpers

        services!.AddSingleton<IActivationService, ActivationService>();
        services!.AddSingleton<IPageService, PageService>();
        services!.AddSingleton<INavigationService, NavigationService>();
        services!.AddTransient<INavigationViewService, NavigationViewService>();

        services!.AddAutoMapper(typeof(ProcessInforamtionProfile));
        
        // pages

        services!.AddTransient<MainWindow>();
        services!.AddTransient<MetricsPage>();
        services!.AddTransient<ActionsPage>();
        services!.AddTransient<ShellPage>();
        services!.AddTransient<TablePage>();

        // view-models

        services!.AddTransient<TableViewModel>();
        services!.AddTransient<MetricsViewModel>();
        services!.AddTransient<ActionsViewModel>();
        services!.AddTransient<ShellViewModel>();

        services!.AddSingleton<ProcessMetricsHostedService>();
        services!.AddHostedService(provider => provider.GetRequiredService<ProcessMetricsHostedService>());
    }
}
