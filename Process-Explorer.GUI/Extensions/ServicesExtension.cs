using Microsoft.Extensions.DependencyInjection;
using Process_Explorer.BLL.Core.Profiles;
using Process_Explorer.BLL;

namespace Process_Explorer.GUI.Extensions
{
    public static class ServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection? services)
        {
            services!.AddSingleton<MainWindow>();
            services!.AddAutoMapper(typeof(ProcessInforamtionProfile));
            services!.AddTransient<Tester>();
        }
    }
}
