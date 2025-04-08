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

namespace PreLaunchTaskr.GUI.WinUI3;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(32, 128, 128, 128);
        AppWindow.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(16, 128, 128, 128);

        if (!MicaController.IsSupported())
            SystemBackdrop = new DesktopAcrylicBackdrop();

        hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
    }

    public readonly nint hWnd;

    private readonly ObservableCollection<InfoBarItem> infos = [];

    private void Frame_Loaded(object sender, RoutedEventArgs e)
    {
        Frame frame = (Frame) sender;
        frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
    }
}
