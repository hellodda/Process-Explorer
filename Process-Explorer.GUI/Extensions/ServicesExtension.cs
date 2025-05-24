using Microsoft.Extensions.DependencyInjection;
using Process_Explorer.BLL;
using Process_Explorer.GUI.ViewModels;
using Process_Explorer.BLL.Services;
using Process_Explorer.BLL.Profiles;
using Process_Explorer.GUI.Views;
using Process_Explorer.BLL.HostedServices;

namespace Process_Explorer.GUI.Extensions
{
    public static class ServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection? services)
        {
            services!.AddTransient<IProcessService, ProcessService>();
            services!.AddTransient<ProcessListViewModel>();
            services!.AddTransient<MetricsViewModel>();
            services!.AddTransient<ActionsViewModel>();
            services!.AddAutoMapper(typeof(ProcessInforamtionProfile));
            services!.AddTransient<Tester>();
            services!.AddTransient<MainWindow>();
            services!.AddTransient<MetricsPage>();
            services!.AddTransient<ActionsPage>();
            services!.AddSingleton<ProcessMetricsHostedService>();
            services!.AddHostedService(provider => provider.GetRequiredService<ProcessMetricsHostedService>());
        }
    }
}
