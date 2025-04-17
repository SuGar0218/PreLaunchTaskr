using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Services;

using System.IO;
using System.Windows;

namespace PreLaunchTaskr.GUI.WPF;

public partial class App : Application
{
    public App()
    {
        Configurator = Configurator.Init(GlobalProperties.SettingsLocation, GlobalProperties.LauncherNet6Location);
        Launcher = Launcher.Init(BaseDirectory);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
    }

    public new MainWindow MainWindow => (MainWindow) Application.Current.MainWindow;

    public static new App Current => (App) Application.Current;

    public static string BaseDirectory { get; } = Path.GetFullPath(".")!;

    internal Configurator Configurator { get; private set; } = null!;

    internal Launcher Launcher { get; private set; } = null!;
}
