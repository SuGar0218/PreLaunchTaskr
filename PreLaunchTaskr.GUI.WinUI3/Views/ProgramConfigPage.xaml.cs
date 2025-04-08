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
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Controls;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using System.Threading.Tasks;
using System.Diagnostics;

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

    private void CategoryNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        ProgramConfigCategoryItem selectedItem = (ProgramConfigCategoryItem) args.SelectedItem;
        ContentFrame.Navigate(selectedItem.PageType, selectedItem.ViewModel, args.RecommendedNavigationTransitionInfo);
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        CategoryNavigation.SelectedItem = viewModel.Categories.First();
    }

    private ProgramConfigViewModel viewModel = null!;

    private void Launch_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        Process.Start(viewModel.ProgramListItem.Path);
    }

    private void LaunchAsAdmin_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        Process process = new();
        process.StartInfo.FileName = viewModel.ProgramListItem.Path;
        process.StartInfo.Verb = "runas";
        process.Start();
    }
}
