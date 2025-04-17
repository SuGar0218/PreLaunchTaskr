using PreLaunchTaskr.GUI.WPF.Views;

using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace PreLaunchTaskr.GUI.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        windowInterop = new WindowInteropHelper(this);

        if (SystemParameters.IsGlassEnabled)
        {
            windowChrome = new()
            {
                GlassFrameThickness = new Thickness(-1),
                CaptionHeight = 36
            };
            WindowChrome.SetWindowChrome(this, windowChrome);
        }
        else
        {
            ContentFrame.Background = SystemColors.WindowBrush;
        }
        if ((Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 10))
        {
            if (windowChrome is not null)
            {
                windowChrome.NonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom;
            }
            Color themeColor = SystemParameters.WindowGlassColor;
            Color themeColor16 = SystemParameters.WindowGlassColor;
            themeColor16.A = 16;
            //Background = new SolidColorBrush(themeColor16);
            Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.Transparent, 0.8),
                    new GradientStop(themeColor, 0),
                }
            };
            //#endif
        }
    }

    public bool ExtendsContentIntoTitleBar => windowChrome is not null;

    private void Frame_Loaded(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new MainPage());
    }

    private readonly WindowInteropHelper windowInterop;

    private readonly WindowChrome? windowChrome;
}