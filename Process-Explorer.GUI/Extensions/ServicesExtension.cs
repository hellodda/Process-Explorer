using Microsoft.Extensions.DependencyInjection;
using Process_Explorer.BLL;
using Process_Explorer.GUI.ViewModels;
using Process_Explorer.BLL.Services;
using Process_Explorer.BLL.Profiles;

namespace Process_Explorer.GUI.Extensions
{
    public static class ServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection? services)
        {
            services!.AddTransient<IProcessService, ProcessService>();
            services!.AddTransient<ProcessListViewModel>();
            services!.AddAutoMapper(typeof(ProcessInforamtionProfile));
            services!.AddTransient<Tester>();
            services!.AddSingleton<MainWindow>();
        }
    }
}
