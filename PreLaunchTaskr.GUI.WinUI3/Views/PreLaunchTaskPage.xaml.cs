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

/// <summary>
/// 此页接收导航参数为自身对应的 ViewModel，类型为 PreLaunchTaskViewModel
/// </summary>
public sealed partial class PreLaunchTaskPage : Page
{
    public PreLaunchTaskPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (PreLaunchTaskViewModel) e.Parameter;
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync(DispatcherQueue);
        ListLoadingProgressBar.Visibility = Visibility.Collapsed;
        base.OnNavigatedTo(e);
    }

    private void ConfirmDeleteArgument_Click(object sender, RoutedEventArgs e)
    {
        PreLaunchTaskListItem item = DataContextHelper.GetDataContext<PreLaunchTaskListItem>(sender);
        viewModel.RemoveTask(item);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private PreLaunchTaskViewModel viewModel = null!;
}
