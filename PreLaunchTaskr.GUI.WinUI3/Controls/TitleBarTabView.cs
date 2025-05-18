using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using PreLaunchTaskr.GUI.WinUI3.Extensions;
using PreLaunchTaskr.GUI.WinUI3.Helpers;

using System;
using System.ComponentModel;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public partial class TitleBarTabView : TabView
{
    public TitleBarTabView() : base()
    {
        tabStripFooterBorder = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
        base.TabStripFooter = tabStripFooterBorder;
        RegisterPropertyChangedCallback(TabView.VisibilityProperty, (o, e) => VisibilityChanged?.Invoke(this, EventArgs.Empty));
    }

    public void UpdateTitleBar()
    {
        OwnerWindow?.SetTitleBar(this);
        titleBarPassthroughHelper?.Passthrough(this);
    }

    public event EventHandler? VisibilityChanged;

    public Window? OwnerWindow
    {
        get => (Window?) GetValue(OwnerWindowProperty);
        set
        {
            if (value == OwnerWindow)
                return;

            SetValue(OwnerWindowProperty, value);
            
            if (value is null)
            {
                titleBarPassthroughHelper = null;
                SizeChanged -= PassthroughTitleBarOnSizeChanged;
                tabStripFooterBorder.SizeChanged -= PassthroughTitleBarOnTabStripFooterSizeChanged;
                return;
            }
            titleBarPassthroughHelper = TitleBarPassthroughHelper.For(value);
            tabStripFooterBorder.MinWidth = FlowDirection switch
            {
                FlowDirection.LeftToRight => value.AppWindow.TitleBar.RightInset / XamlRoot.RasterizationScale,
                FlowDirection.RightToLeft => value.AppWindow.TitleBar.LeftInset / XamlRoot.RasterizationScale,
                _ => throw new ArgumentOutOfRangeException()
            };
            VisibilityChanged += (o, e) =>
            {
                if (Visibility == Visibility.Visible)
                {
                    OwnerWindow?.SetTitleBar(this);
                    titleBarPassthroughHelper.Passthrough(this);
                }
            };
            SizeChanged += PassthroughTitleBarOnSizeChanged;
            tabStripFooterBorder.SizeChanged += PassthroughTitleBarOnTabStripFooterSizeChanged;
        }
    }

    public bool AutoHide
    {
        get => (bool) GetValue(AutoHideProperty);
        set
        {
            SetValue(AutoHideProperty, value);

            if (value)
            {
                TabItemsChanged += ShowOrHideOnTabItemsChanged;
            }
            else
            {
                TabItemsChanged -= ShowOrHideOnTabItemsChanged;
            }
        }
    }

    public new UIElement? TabStripFooter
    {
        get => (UIElement?) GetValue(TabStripFooterProperty);
        set => SetValue(TabStripFooterProperty, value);
    }

    public Action<FrameworkElement>? PassthroughAlternativeTitleBar;

    public static readonly DependencyProperty OwnerWindowProperty = DependencyProperty.Register
    (
        nameof(OwnerWindow),
        typeof(Window),
        typeof(TitleBarTabView),
        new PropertyMetadata(default(Window))
    );

    public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register
    (
        nameof(AutoHide),
        typeof(bool),
        typeof(TitleBarTabView),
        new PropertyMetadata(false)
    );

    public static new readonly DependencyProperty TabStripFooterProperty = DependencyProperty.Register
    (
        nameof(TabStripFooter),
        typeof(UIElement),
        typeof(TitleBarTabView),
        new PropertyMetadata(default, (d, e) =>
        {
            TitleBarTabView self = (TitleBarTabView) d;
            self.tabStripFooterBorder.Child = (UIElement) e.NewValue;
        })
    );

    protected readonly Border tabStripFooterBorder;

    private TitleBarPassthroughHelper? titleBarPassthroughHelper;

    private bool isSizeChanged;  // SizeChanged 发生先于子元素

    private void ShowOrHideOnTabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
    {
        sender.Visibility = sender.TabItems.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PassthroughTitleBarOnTabStripFooterSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!isSizeChanged)
        {
            OwnerWindow?.SetTitleBar(this);
            titleBarPassthroughHelper?.Passthrough(this);
        }
        isSizeChanged = false;
    }

    private void PassthroughTitleBarOnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        isSizeChanged = true;
        OwnerWindow?.SetTitleBar(this);
        titleBarPassthroughHelper?.Passthrough(this);
    }
}
