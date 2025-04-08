using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public sealed partial class IconHyperlinkButton : HyperlinkButton
{
    public IconHyperlinkButton()
    {
        InitializeComponent();
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
}
