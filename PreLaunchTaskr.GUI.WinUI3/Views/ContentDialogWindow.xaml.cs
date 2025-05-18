using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class ContentDialogWindow : Window
{
    public ContentDialogWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        OverlappedPresenter presenter = (OverlappedPresenter) AppWindow.Presenter;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
    }

    public new object? Content { get; set; }

    public string PrimaryButtonText { get; set; } = string.Empty;
    public string SecondaryButtonText { get; set; } = string.Empty;
    public string CloseButtonText { get; set; } = string.Empty;

    public bool IsPrimaryButtonEnabled { get; set; } = true;
    public bool IsSecondaryButtonEnabled { get; set; } = true;

    private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = (ContentDialog) sender;
        AppWindow.Resize(new Windows.Graphics.SizeInt32(
            (int) (dialog.ActualWidth / dialog.XamlRoot.RasterizationScale),
            (int) (dialog.ActualHeight / dialog.XamlRoot.RasterizationScale)));
        Activate();
        await dialog.ShowAsync();
        AppWindow.Hide();
        Close();
    }
}
