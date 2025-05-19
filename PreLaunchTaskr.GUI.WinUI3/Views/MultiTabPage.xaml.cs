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
    /// ʱ�临�Ӷ�����Ϊ O(n)
    /// </summary>
    /// <param name="item">������ӵı�ǩҳ��</param>
    /// <param name="areSame">�ж�������ǩҳ����ͬ�ķ���</param>
    /// <returns>�Ƿ���ӳɹ�</returns>
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
            ContentFrame.Navigate(typeof(Page), null, new SuppressNavigationTransitionInfo());
            return;
        }

        TabStripItem item = (TabStripItem) e.AddedItems[0];
        ContentFrame.Navigate(item.PageType, item.ExtraData, new SuppressNavigationTransitionInfo());
        ContentFrame.BackStack.Clear();
    }

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = TitleBarPassthroughHelper.For(App.Current.MainWindow);

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
        RemoveTabStripItem((TabStripItem) args.Item);
    }

    private void ShowContextMenuOnTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement self)
            return;

        self.ContextFlyout?.ShowAt(self);
    }
}
