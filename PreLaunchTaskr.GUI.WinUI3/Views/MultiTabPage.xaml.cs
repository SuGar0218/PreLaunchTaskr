using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        TabStripShown += (o, e) =>
        {
            TabStrip.UpdateTitleBar();
        };

        App.Current.MultiTab = this;
    }

    public Visibility TabStripVisibility => TabStrip.Visibility;

    public event EventHandler? TabStripShown;
    public event EventHandler? TabStripHidden;

    public Action<FrameworkElement>? PassthroughAlternativeTitleBar
    {
        get => TabStrip.PassthroughAlternativeTitleBar;
        set => TabStrip.PassthroughAlternativeTitleBar = value;
    }

    public ObservableCollection<TabStripItem> TabStripItems { get; } = [];

    public int CurrentTabIndex
    {
        get => TabStrip.SelectedIndex;
        set => TabStrip.SelectedIndex = value;
    }

    public TabStripItem? CurrentTabItem
    {
        get => (TabStripItem?) TabStrip.SelectedItem;
        set => TabStrip.SelectedItem = value;
    }

    public void AddTabStripItem(TabStripItem item, bool select = true)
    {
        TabStripItems.Add(item);
        if (select)
        {
            CurrentTabIndex = TabStripItems.Count - 1;
            CurrentTabItem = item;
        }
    }

    public void RemoveTabStripItem(TabStripItem item)
    {
        int index = TabStripItems.IndexOf(item);
        if (index == CurrentTabIndex)
        {
            CurrentTabIndex--;
            CurrentTabItem = CurrentTabIndex < 0 ? null : TabStripItems[CurrentTabIndex];
        }
        TabStripItems.RemoveAt(index);
    }

    /// <summary>
    /// 时间复杂度至少为 O(n)
    /// </summary>
    /// <param name="item">尝试添加的标签页项</param>
    /// <param name="areSame">判断两个标签页项相同的方法</param>
    /// <returns>是否添加成功</returns>
    public bool TryAddUniqueTabStripItem(TabStripItem item, Func<TabStripItem, TabStripItem, bool> areSame, bool select = true)
    {
        int i = 0;
        foreach (TabStripItem existed in TabStripItems)
        {
            if (areSame(item, existed))
            {
                if (select)
                {
                    CurrentTabIndex = i;
                    CurrentTabItem = existed;
                }
                return false;
            }
            i++;
        }

        AddTabStripItem(item, select);
        return true;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //TabStripFooterSpace.MinWidth = App.Current.MainWindow.AppWindow.TitleBar.RightInset / XamlRoot.RasterizationScale;
        App.Current.MainWindow.SetTitleBar(TabStrip);
        AddTabStripItem(new TabStripItem(
            //nameof(PreLaunchTaskr),
            string.Empty,
            new SymbolIconSource { Symbol = Symbol.Home },
            closeable: false,
            typeof(MainPage),
            new MainViewModel()));

        ContentFrame.SizeChanged += (o, e) =>
        {
            if (e.NewSize.Height > e.PreviousSize.Height)
            {
                TabStripHidden?.Invoke(this, EventArgs.Empty);
            }
            else if (e.NewSize.Height < e.PreviousSize.Height)
            {
                TabStripShown?.Invoke(this, EventArgs.Empty);
            }
        };
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
            ContentFrame.Navigate(typeof(Page), null, new SuppressNavigationTransitionInfo());
            return;
        }

        TabStripItem item = (TabStripItem) e.AddedItems[0];
        ContentFrame.Navigate(item.PageType, item.ExtraData, new SuppressNavigationTransitionInfo());
        ContentFrame.BackStack.Clear();
    }

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = TitleBarPassthroughHelper.For(App.Current.MainWindow);

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
        RemoveTabStripItem((TabStripItem) args.Item);
    }

    private void ShowContextMenuOnTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement self)
            return;

        self.ContextFlyout?.ShowAt(self);
    }
}
