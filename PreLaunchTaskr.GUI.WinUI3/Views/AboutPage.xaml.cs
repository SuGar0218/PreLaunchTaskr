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
@"���� Windows 11 �ķ���������ͼ�������Ѵ� Segoe MDL2 Assets �滻Ϊ Segoe Fluent Icons ���塣

�� Windows 10 �����Ͽ��ܲ���ͼ�겻��������ʾ��������ʾΪһ�����ο�������Ҫ�������أ�Ȼ�����Ҽ��˵���Ϊ�����û���װ��";
}
