using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Process_Explorer.GUI.Extensions;
using Process_Explorer.GUI.Helpers;
using System;

namespace Process_Explorer.GUI
{
    public partial class App : Application
    {
        private IHost _host = default!;
        private Window _window = default!;

        public App()
        {
            InitializeComponent();
            SetupApplication();
            SetupHost();
        }

        private void SetupHost()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureServices((ctx, services) => services.ConfigureServices())
                .Build();
        }

        private void SetupApplication()
        {
            try
            {
                Native.Application.SetPrivileges();
                Native.Application.SetPriority();
            }
            catch (Exception ex)
            {
                ToastNotificationHelper.ShowMessage("Process Explorer Message", ex.Message);
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await _host.StartAsync();

            _window = _host.Services.GetRequiredService<MainWindow>();
            _window.Activate();

            _window.Closed += OnMainWindowClosed;
        }

        private async void OnMainWindowClosed(object? sender, WindowEventArgs e)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
        }
    }
}