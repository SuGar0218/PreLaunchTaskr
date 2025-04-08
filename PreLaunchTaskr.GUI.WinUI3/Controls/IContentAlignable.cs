using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public interface IContentAlignable<TPanel> where TPanel : Panel
{
    public abstract TPanel Self { get; }

    public void RefreshLayout()
    {
        foreach (UIElement element in Self.Children)
        {
            FrameworkElement? frameworkElement = element as FrameworkElement;
            if (frameworkElement is not null)
            {
                frameworkElement.HorizontalAlignment = ContentHorizontalAlignment;
                frameworkElement.VerticalAlignment = ContentVerticalAlignment;
            }
        }
    }

    public HorizontalAlignment ContentHorizontalAlignment
    {
        get => (HorizontalAlignment) Self.GetValue(ContentHorizontalAlignmentProperty);
        set => Self.SetValue(ContentHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment ContentVerticalAlignment
    {
        get => (VerticalAlignment) Self.GetValue(ContentVerticalAlignmentProperty);
        set => Self.SetValue(ContentVerticalAlignmentProperty, value);
    }

    public static DependencyProperty ContentHorizontalAlignmentProperty { get; } = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(HorizontalAlignment),
        typeof(MyStackPanel),
        new PropertyMetadata(default));

    public static DependencyProperty ContentVerticalAlignmentProperty { get; } = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(VerticalAlignment),
        typeof(MyStackPanel),
        new PropertyMetadata(default));
}
