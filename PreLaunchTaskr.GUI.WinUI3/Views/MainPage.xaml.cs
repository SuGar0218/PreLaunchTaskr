using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Extensions;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Win32;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        Navigation.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, (o, dp) => AddProgramButton.IsCompact = !Navigation.IsPaneOpen);
    }

    private readonly MainViewModel viewModel = new();

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = new(App.Current.MainWindow);

    private void AddProgramButton_Click(object sender, RoutedEventArgs e)
    {
        AddProgramMenu.ShowAt(AddProgramButton);
    }

    private async void OpenProgramFile_Click(object sender, RoutedEventArgs e)
    {
        StorageFile? file = await FileOpenPickerHelper
            .OpenFileForWindow(App.Current.MainWindow)
            .AddFileTypeFilter(".exe")
            .PickSingleAsync();

        if (file is null)
            return;

        if (!viewModel.AddProgram(file.Name, file.Path))
        {
            await this.MessageBox("注册表中的映像劫持是按照文件名而不是文件路径，因此即使不同位置同名文件，也会被映像劫持。", $"已添加过同名文件 {file.Name}");
        }
    }

    private void InputProgramPath_Click(object sender, RoutedEventArgs e)
    {
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ProgramListProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync(DispatcherQueue);
        ProgramListProgressBar.Visibility = Visibility.Collapsed;
        App.Current.MainWindow.ExtendsContentIntoTitleBar = true;
        App.Current.MainWindow.SetTitleBar(TitleBarBorder);
        titleBarPassthroughHelper.Passthrough(TitleBarToggleButton);
        AddProgramButton.MinWidth = Navigation.CompactPaneLength - 8;  // 折叠的左侧导航菜单宽度减两边的 Margin
        // <Thickness x:Key="NavigationViewItemButtonMargin">4,2</Thickness>
        // https://github.com/microsoft/microsoft-ui-xaml/blob/main/src/controls/dev/NavigationView/NavigationView_themeresources.xaml

        ContentFrame.Navigate(typeof(ProgramUnselectedPage));
    }

    private void TitleBarToggleButton_Click(object sender, RoutedEventArgs e)
    {
        Navigation.IsPaneOpen = !Navigation.IsPaneOpen;
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is null)
        {
            ContentFrame.Navigate(typeof(ProgramUnselectedPage));
            return;
        }

        ContentFrame.Navigate(typeof(ProgramConfigPage), args.SelectedItem);
    }

    private void RemoveProgram_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        viewModel.RemoveProgram(item);
    }

    private void CopyProgramPath_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        ClipboardHelper.Copy(item.Path);
    }

    private void OpenProgramPath_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        WindowsHelper.OpenPathInExplorer(item.Path);
    }
}
