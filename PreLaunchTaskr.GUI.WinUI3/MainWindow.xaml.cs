using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Views;

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

        if (MicaController.IsSupported())
        {
            SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
        }
        else if (DesktopAcrylicController.IsSupported())
        {
            systemBackdropConfiguration = new SystemBackdropConfiguration();
            desktopAcrylicController = new DesktopAcrylicController
            {
                Kind = DesktopAcrylicKind.Thin,
                LuminosityOpacity = 0.5f
            };
            desktopAcrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
            desktopAcrylicController.SetSystemBackdropConfiguration(systemBackdropConfiguration);
            Activated += (o, e) =>
            {
                systemBackdropConfiguration.IsInputActive = e.WindowActivationState != WindowActivationState.Deactivated;
            };
            Color themeColor = new UISettings().GetColorValue(UIColorType.Accent);
            themeColor.A = 32;
            Background = new SolidColorBrush(themeColor);
        }
        else
        {
            Color themeColor = new UISettings().GetColorValue(UIColorType.Accent);
            themeColor.A = 32;
            Background = new SolidColorBrush(themeColor);
        }

        hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        MaterialController = new(this);
    }

    public Brush Background
    {
        get => ContentFrame.Background;
        set => ContentFrame.Background = value;
    }

    public readonly nint hWnd;

    public MainWindowMaterialController MaterialController { get; }

    private void Frame_Loaded(object sender, RoutedEventArgs e)
    {
        Frame frame = (Frame) sender;
        frame.Navigate(typeof(MultiTabPage), null);

        double scale = frame.XamlRoot.RasterizationScale;
        AppWindow.Resize(new Windows.Graphics.SizeInt32((int) (3 * NavigationViewOpenPaneLength * scale), (int) (720 * scale)));

        Activate();
    }

    private void Window_Closed(object o, WindowEventArgs e)
    {
        desktopAcrylicController?.RemoveAllSystemBackdropTargets();
        desktopAcrylicController?.Dispose();
    }

    private const double NavigationViewOpenPaneLength = 320;

    private readonly DesktopAcrylicController? desktopAcrylicController;
    private readonly SystemBackdropConfiguration? systemBackdropConfiguration;
}
