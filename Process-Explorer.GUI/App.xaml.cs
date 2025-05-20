using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
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
        }

        private Window? m_window;
    }
}
