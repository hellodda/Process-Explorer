using Microsoft.Extensions.DependencyInjection;
using Process_Explorer.BLL.Core.Profiles;
using Process_Explorer.BLL;
using Process_Explorer.BLL.Core.Services;
using Process_Explorer.GUI.ViewModels;

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
