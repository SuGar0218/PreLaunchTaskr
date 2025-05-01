using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class MultiTabPage : Page
{
    public MultiTabPage()
    {
        InitializeComponent();
        App.Current.MultiTab = viewModel;
    }

    private readonly MultiTabViewModel viewModel = new();

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        TabStripFooterSpace.MinWidth = App.Current.MainWindow.AppWindow.TitleBar.RightInset / XamlRoot.RasterizationScale + 16;
        App.Current.MainWindow.SetTitleBar(TabStrip);
        viewModel.AddTabStripItem(new TabStripItem(
            nameof(PreLaunchTaskr),
            new SymbolIconSource { Symbol = Symbol.Home },
            closeable: false,
            typeof(MainPage),
            new MainViewModel()));
    }

    /// <summary>
    /// TabItems �����ı�ʱ�����Ի�ȡ TabItem �� Container ʼ��Ϊ null����˲��� Passthrough
    /// </summary>
    private void TabStrip_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
    {
        if (args.CollectionChange == CollectionChange.ItemRemoved && sender.TabItems.Count == 0)
        {
            Application.Current.Exit();
            return;
        }
    }

    private void TabStrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] is null)
        {
            ContentFrame.Navigate(typeof(Page));
            return;
        }

        TabStripItem item = (TabStripItem) e.AddedItems[0];
        ContentFrame.Navigate(item.PageType, item.ExtraData);
    }

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = new(App.Current.MainWindow);

    // TabView.SizeChanged ������ TabView.TabStripFooter �ڵ� SizeChanged ����

    private bool tabStripSizeChanged;

    /// <summary>
    /// ��ǩҳ���ߴ�仯ʱ����Ҫ����ִ�б����� Passthrough
    /// </summary>
    private void TabStrip_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        tabStripSizeChanged = true;
        titleBarPassthroughHelper.Passthrough(TabStrip);
    }

    /// <summary>
    /// ��ǩҳ��ĩ�˳ߴ�仯ʱ����Ҫ����ִ�б����� Passthrough��
    /// ����������������ߴ��Ѿ��仯���ˣ���û��Ҫ��ִ�С�
    /// </summary>
    private void TabStripFooterSpace_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!tabStripSizeChanged)
            titleBarPassthroughHelper.Passthrough(TabStrip);

        tabStripSizeChanged = false;
    }

    private void TabStrip_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        //sender.TabItems.Remove(args.Tab);  // ���� ItemsSource������ʹ�����
        viewModel.RemoveTabStripItem((TabStripItem) args.Item);
    }
}
