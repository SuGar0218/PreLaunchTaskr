using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Extensions;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Diagnostics;
using System.Linq;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 接收导航参数的类型为 programListItem
/// </summary>
public sealed partial class ProgramConfigPage : Page
{
    public ProgramConfigPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 接收导航参数的类型为 ProgramInfo
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = new ProgramConfigViewModel((ProgramListItem) e.Parameter);
        base.OnNavigatedTo(e);
    }

    private ProgramConfigCategoryItem? lastSelectedItem = null;

    private void CategoryNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        ProgramConfigCategoryItem selectedItem = (ProgramConfigCategoryItem) args.SelectedItem;
        NavigationTransitionInfo? transition;
        if (lastSelectedItem is null || lastSelectedItem == selectedItem)
        {
            transition = new SuppressNavigationTransitionInfo();
        }
        else
        {
            transition = args.RecommendedNavigationTransitionInfo;
        }
        ContentFrame.Navigate(
            selectedItem.PageType,
            selectedItem.PageViewModel,
            transition);
        lastSelectedItem = selectedItem;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        CategoryNavigation.SelectedItem ??= viewModel.Categories.First();
    }

    private ProgramConfigViewModel viewModel = null!;

    private async void Launch_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(viewModel.ProgramListItem.Path);
        }
        catch (Exception exception)
        {
            await this.MessageBox(exception.Message, exception.GetType().Name);
        }
    }

    private async void LaunchAsAdmin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process process = new();
            process.StartInfo.FileName = viewModel.ProgramListItem.Path;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.Start();
        }
        catch (Exception exception)
        {
            await this.MessageBox(exception.Message, exception.GetType().Name);
        }
    }

    private void CopyFileName_Click(object sender, RoutedEventArgs e)
    {
        ClipboardHelper.Copy(viewModel.Name);
    }

    private void CopyFilePath_Click(object sender, RoutedEventArgs e)
    {
        ClipboardHelper.Copy(viewModel.Path);
    }

    private void SaveIcon_Click(object sender, RoutedEventArgs e)
    {

    }

    private void GoToFilePath_Click(object sender, RoutedEventArgs e)
    {
        WindowsHelper.OpenPathInExplorer(viewModel.Path);
    }
}
