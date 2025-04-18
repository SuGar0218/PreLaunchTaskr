using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Helpers;

public static class LayoutHelper
{
    public static void VerticalAlignContent(Panel panel, VerticalAlignment alignment)
    {
        foreach (UIElement element in panel.Children)
        {
            ((FrameworkElement) element).VerticalAlignment = alignment;
        }
    }

    public static void HorizontalAlignContent(Panel panel, HorizontalAlignment alignment)
    {
        foreach (UIElement element in panel.Children)
        {
            ((FrameworkElement) element).HorizontalAlignment = alignment;
        }
    }

    public static void PanelCenterVerticalAlignContentOnLoaded(object panel, RoutedEventArgs e)
    {
        VerticalAlignContent((Panel) panel, VerticalAlignment.Center);
    }

    public static void PanelCenterHorizontalAlignContentOnLoaded(object panel, RoutedEventArgs e)
    {
        HorizontalAlignContent((Panel) panel, HorizontalAlignment.Center);
    }
}
