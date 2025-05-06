using Microsoft.UI.Xaml.Controls;

using System.IO;
using System.Text;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private string ReadMe
    {
        get
        {
            readme ??= File.ReadAllText(Path.Combine(App.BaseDirectory, "readme.txt"), Encoding.UTF8);
            return readme;
        }
    }

    private static string readme = null!;
}
