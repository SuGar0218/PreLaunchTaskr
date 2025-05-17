using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Views;

using System;
using System.Collections.ObjectModel;

using Windows.UI;
using Windows.UI.ViewManagement;

using WinRT;

namespace PreLaunchTaskr.GUI.WinUI3;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(32, 128, 128, 128);
        AppWindow.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(16, 128, 128, 128);
        OverlappedPresenter presenter = (OverlappedPresenter) AppWindow.Presenter;
        presenter.IsMaximizable = false;
#if DEBUG
        if (false)
#else
        if (MicaController.IsSupported())
#endif
        {
            SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
        }
        else if (DesktopAcrylicController.IsSupported())
        {
            //SystemBackdrop = new DesktopAcrylicBackdrop();
            systemBackdropConfiguration = new SystemBackdropConfiguration();
            desktopAcrylicController = new DesktopAcrylicController
            {
                LuminosityOpacity = (MathF.Sqrt(5) - 1) / 2
            };
            desktopAcrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
            desktopAcrylicController.SetSystemBackdropConfiguration(systemBackdropConfiguration);
            Activated += (o, e) =>
            {
                systemBackdropConfiguration.IsInputActive = e.WindowActivationState != WindowActivationState.Deactivated;
                if (e.WindowActivationState == WindowActivationState.Deactivated)
                {
                    CurrentBackground = InactiveBackground;
                }
                else
                {
                    CurrentBackground = ActiveBackground;
                }
            };
            Color themeColor = new UISettings().GetColorValue(UIColorType.Accent);
            themeColor.A = 48;
            ActiveBackground = new SolidColorBrush(themeColor);
        }
        else
        {
            Color themeColor = new UISettings().GetColorValue(UIColorType.Accent);
            themeColor.A = 48;
            ActiveBackground = new SolidColorBrush(themeColor);
        }

        hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        MaterialController = new(this);
    }

    public Brush? ActiveBackground
    {
        get => field;
        set => field = value;
    }

    public Brush? InactiveBackground
    {
        get => field;
        set => field = value;
    }

    private Brush? CurrentBackground
    {
        get => ContentFrame.Background;
        set => ContentFrame.Background = value;  // 设置当前 Frame 的背景色，只有在 MainWindow 加载完毕后，ContentFrame 才不为 null
    }

    public readonly nint hWnd;

    public MainWindowMaterialController MaterialController { get; }

    private void Frame_Loaded(object sender, RoutedEventArgs e)
    {
        Frame frame = (Frame) sender;
        frame.Navigate(typeof(MultiTabPage));

        double scale = frame.XamlRoot.RasterizationScale;
        AppWindow.Resize(new Windows.Graphics.SizeInt32((int) (3 * NavigationViewOpenPaneLength * scale), (int) (720 * scale)));

        Activate();  // 因为把激活窗口延后到了 Frame 加载完毕，所以没在设置 CurrentBackground 时引发空引用异常。
    }

    private void Window_Closed(object o, WindowEventArgs e)
    {
        AppWindow.Hide();
        desktopAcrylicController?.RemoveAllSystemBackdropTargets();
        desktopAcrylicController?.Dispose();
    }

    private const double NavigationViewOpenPaneLength = 320;

    private readonly DesktopAcrylicController? desktopAcrylicController;
    private readonly SystemBackdropConfiguration? systemBackdropConfiguration;

    private static ThemeShadow? ThemeShadowOnMainWindow
    {
        get
        {
            field ??= new ThemeShadow();
            return field;
        }
    }
}
