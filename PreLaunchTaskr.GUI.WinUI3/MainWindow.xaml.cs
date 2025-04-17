using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Views;

using System.Collections.ObjectModel;

using Windows.UI;

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

        if (!MicaController.IsSupported() && DesktopAcrylicController.IsSupported())
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
        }
        else
        {
            SystemBackdrop = new MicaBackdrop();
        }

        hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
    }

    public readonly nint hWnd;

    //private readonly ObservableCollection<InfoBarItem> infos = [];

    private void Frame_Loaded(object sender, RoutedEventArgs e)
    {
        Frame frame = (Frame) sender;
        frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());

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
