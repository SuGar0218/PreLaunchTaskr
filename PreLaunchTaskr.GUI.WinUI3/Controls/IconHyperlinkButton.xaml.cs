using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public sealed partial class IconHyperlinkButton : HyperlinkButton
{
    public IconHyperlinkButton()
    {
        InitializeComponent();
        Thickness padding = Padding;
        padding.Left = 0;
        padding.Right = 0;
        Padding = padding;
    }

    public IconSource? IconSource
    {
        get => (IconSource?) GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsCompact
    {
        get => (bool) GetValue(IsComapctProperty);
        set => SetValue(IsComapctProperty, value);
    }

    public HorizontalAlignment ContentHorizontalAlignment
    {
        get => (HorizontalAlignment) GetValue(ContentHorizontalAlignmentProperty);
        set => SetValue(ContentHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment ContentVerticalAlignment
    {
        get => (VerticalAlignment) GetValue(ContentVerticalAlignmentProperty);
        set => SetValue(ContentVerticalAlignmentProperty, value);
    }

    public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
        nameof(IconSource),
        typeof(IconSource),
        typeof(IconHyperlinkButton),
        new PropertyMetadata(default));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(IconHyperlinkButton),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsComapctProperty = DependencyProperty.Register(
        nameof(IsCompact),
        typeof(bool),
        typeof(IconHyperlinkButton),
        new PropertyMetadata(default, static (d, e) =>
        {
            IconHyperlinkButton iconHyperlinkButton = (IconHyperlinkButton) d;
            iconHyperlinkButton.TextBlock.Visibility = (bool) e.NewValue ? Visibility.Collapsed : Visibility.Visible;
        }));

    public static readonly DependencyProperty ContentHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(HorizontalAlignment),
        typeof(IconHyperlinkButton),
        new PropertyMetadata(default));

    public static readonly DependencyProperty ContentVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(VerticalAlignment),
        typeof(IconHyperlinkButton),
        new PropertyMetadata(default));
}
