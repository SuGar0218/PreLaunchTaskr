using Microsoft.UI.Xaml;

using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Services;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Diagnostics;
using System.IO;

using Windows.Win32;
using Windows.Win32.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PreLaunchTaskr.GUI.WinUI3;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static string DisplayVersion =>
#if DEBUG
        "1.4.5 DEBUG";
#else
        "1.4.5";
#endif

    /// <summary>
    /// Initializes the singleton application object.
    /// This is the first line of authored code executed,
    /// and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
#if !DEBUG
        // 只能开一个窗口
        if (mainWindow is not null)
        {
            mainWindow.Activate();
            Exit();
            return;
        }
        if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GlobalProperties.WinUI3Location)).Length > 1)
        {
            Exit();
            return;
        }
        // 这里结束之后仍然会触发 OnLaunched
#endif

        InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    public static new App Current => (App) Application.Current;

    public MainWindow MainWindow => mainWindow;

    public MultiTabViewModel MultiTab { get; set; } = null!;

    public static string BaseDirectory { get; } = Path.GetFullPath(".");

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if !DEBUG
        // 只能开一个窗口
        if (mainWindow is not null)
        {
            mainWindow.Activate();
            Exit();
            return;
        }
        if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GlobalProperties.WinUI3Location)).Length > 1)
        {
            Exit();
            return;
        }
#endif

        Configurator = Configurator.Init(GlobalProperties.SettingsLocation, GlobalProperties.LauncherNet8Location);
        Launcher = Launcher.Init(BaseDirectory);
        mainWindow = new MainWindow();
        // WinUI 3 默认的窗口启动大小太大，
        // 而 AppWindow 的 Resize 设置的是物理像素，而不是 DIP，
        // 因此等到 MainWindow 中的根元素加载完毕时，才能计算大小，
        // MainWindow 会在根元素 Frame 加载完毕时显示。
        //MainWindow.Activate();
        //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        System.Exception exception = (System.Exception) e.ExceptionObject;
        PInvoke.MessageBox(
            new HWND(App.Current.MainWindow.hWnd),
            exception.Message + "\n\n" + exception.StackTrace,
            exception.Message,
            Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONERROR);
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        PInvoke.MessageBox(
            new HWND(App.Current.MainWindow.hWnd),
            e.Exception.Message + "\n\n" + e.Exception.StackTrace,
            e.Exception.Message,
            Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONERROR);
    }

    private void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        PInvoke.MessageBox(
            new HWND(App.Current.MainWindow.hWnd),
            e.Exception.Message + "\n\n" + e.Exception.StackTrace,
            e.Exception.Message,
            Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONERROR);
    }

    internal Configurator Configurator { get; private set; } = null!;
    internal Launcher Launcher { get; private set; } = null!;

    private static MainWindow mainWindow = null!;
}
