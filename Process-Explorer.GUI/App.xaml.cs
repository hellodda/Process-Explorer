using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Process_Explorer.GUI.Contracts.Services;
using Process_Explorer.GUI.Extensions;
using Process_Explorer.GUI.Helpers;
using System;

namespace Process_Explorer.GUI;

public partial class App : Application
{
    public IHost Host { get; private set; } = default!;
    public static Window Window { get; set; } = new();

    public App()
    {
        InitializeComponent();
        SetupApplication();
        SetupHost();
    }

    public static T GetService<T>() where T : class
    {
        if ((Current as App)!.Host.Services.GetRequiredService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }
        return service;
    }

    private void SetupHost()
    {
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
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

        UnhandledException += App_UnhandledException;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await Host.StartAsync();

        await App.GetService<IActivationService>().ActivateAsync(args);

        Window.Closed += OnMainWindowClosed;
    }

    private async void OnMainWindowClosed(object? sender, WindowEventArgs e)
    {
        await Host.StopAsync(TimeSpan.FromSeconds(5));
        Host.Dispose();
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        ToastNotificationHelper.ShowMessage("Unhandled Exception", e.Message);
    }
}