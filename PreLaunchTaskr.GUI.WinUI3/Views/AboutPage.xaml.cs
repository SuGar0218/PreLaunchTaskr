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

    private string AboutFontIcon
    {
        get => aboutFontIcon;
    }

    private static string readme = null!;

    private const string aboutFontIcon =
@"随着 Windows 11 的发布，符号图标字体已从 Segoe MDL2 Assets 替换为 Segoe Fluent Icons 字体。

在 Windows 10 电脑上可能部分图标不能正常显示，例如显示为一个矩形框。如有需要可以下载，然后在右键菜单中为所有用户安装。";
}
