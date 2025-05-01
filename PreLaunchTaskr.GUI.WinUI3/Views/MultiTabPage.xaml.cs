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
    /// TabItems 发生改变时，尝试获取 TabItem 的 Container 始终为 null，因此不能 Passthrough
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

    // TabView.SizeChanged 会先于 TabView.TabStripFooter 内的 SizeChanged 发生

    private bool tabStripSizeChanged;

    /// <summary>
    /// 标签页栏尺寸变化时，需要重新执行标题栏 Passthrough
    /// </summary>
    private void TabStrip_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        tabStripSizeChanged = true;
        titleBarPassthroughHelper.Passthrough(TabStrip);
    }

    /// <summary>
    /// 标签页栏末端尺寸变化时，需要重新执行标题栏 Passthrough，
    /// 但如果整个标题栏尺寸已经变化过了，那没必要再执行。
    /// </summary>
    private void TabStripFooterSpace_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!tabStripSizeChanged)
            titleBarPassthroughHelper.Passthrough(TabStrip);

        tabStripSizeChanged = false;
    }

    private void TabStrip_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        //sender.TabItems.Remove(args.Tab);  // 绑定了 ItemsSource，不能使用这个
        viewModel.RemoveTabStripItem((TabStripItem) args.Item);
    }
}
