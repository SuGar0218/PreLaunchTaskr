using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

using System;
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

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        ConnectedAnimationService
            .GetForCurrentView()
            .PrepareToAnimate("forwardAnimation", BannerImage);

        base.OnNavigatingFrom(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        ConnectedAnimationService
            .GetForCurrentView()
            .GetAnimation("backAnimation")?
            .TryStart(BannerImage);
    }

    private void BannerImage_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        int r = random.Next();
        if (r % 10 == 0 || r % 10 == 1)
        {
            Frame.Navigate(typeof(BannerPage), null, new SuppressNavigationTransitionInfo());
        }
    }

    private static readonly Random random = new();

    private static string readme = null!;

    private const string aboutFontIcon =
@"���� Windows 11 �ķ���������ͼ�������Ѵ� Segoe MDL2 Assets �滻Ϊ Segoe Fluent Icons ���塣

�� Windows 10 �����Ͽ��ܲ���ͼ�겻��������ʾ��������ʾΪһ�����ο�������Ҫ�������أ�Ȼ�����Ҽ��˵���Ϊ�����û���װ��";
}
