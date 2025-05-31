using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Process_Explorer.BLL.HostedServices;
using Process_Explorer.GUI.Extensions;

namespace Process_Explorer.GUI
{
    public partial class App : Application
    {
        private IServiceProvider _services = default!;

        public App()
        {
          
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            try
            {
                Native.Application.SetPrivileges();
                Native.Application.SetPriority();
            }
            catch (Exception ex)
            {
                Native.MessageBox.ShowWarning(ex.Message);
            }

            var services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();   
                logging.SetMinimumLevel(LogLevel.Debug); 
            });

            services.ConfigureServices();

            _services = services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = _services.GetRequiredService<MainWindow>();
            m_window.Activate();

            var hostedService = _services.GetRequiredService<ProcessMetricsHostedService>();
            hostedService.StartAsync(CancellationToken.None);
        }

        private Window? m_window;
    }
}
