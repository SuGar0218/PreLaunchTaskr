using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Extensions;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        Navigation.IsPaneOpen = false;
        Navigation.ExpandedModeThresholdWidth = Math.E * Navigation.OpenPaneLength;
        Navigation.RegisterPropertyChangedCallback(
            NavigationView.IsPaneOpenProperty,
            (o, dp) => AddProgramButton.IsCompact = !Navigation.IsPaneOpen);
    }

    private readonly MainViewModel viewModel = new();

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = new(App.Current.MainWindow);

    private void AddProgramButton_Click(object sender, RoutedEventArgs e)
    {
        AddProgramMenu.ShowAt(AddProgramButton);
    }

    private async void SelectProgramFile_Click(object sender, RoutedEventArgs e)
    {
        StorageFile? file = await FileOpenPickerHelper
            .OpenFileForWindow(App.Current.MainWindow)
            .AddFileTypeFilter(".exe")
            .PickSingleAsync();

        if (file is null || string.IsNullOrWhiteSpace(file.Path))
            return;

        if (!viewModel.AddProgram(file.Name, file.Path))
        {
            await ShowFileNameExistedMessageBox(file.Name);
        }
    }

    private async void InputProgramPath_Click(object sender, RoutedEventArgs e)
    {
        InputTextPage page = new();
        await this.MessageBox(page, "程序所在位置");
        if (string.IsNullOrWhiteSpace(page.Text))
            return;

        string path = StringHelper.TrimQuotes(page.Text.Trim());

        if (!File.Exists(path))
        {
            await this.MessageBox(path, "没有这个文件");
            return;
        }

        string name = Path.GetFileName(path);

        if (!viewModel.AddProgram(name, path))
            await ShowFileNameExistedMessageBox(name);
    }

    private async void SelectInstalledProgram_Click(object sender, RoutedEventArgs e)
    {
        // 暂不支持商店应用，因为商店应用所在路径无权创建文件，就无法创建符号链接。
        //InstalledProgramListPage page = new();
        //DialogResult result;
        //result = await this.MessageBox(page, "已安装的程序", MessageBoxButtons.OKCancel);
        //if (result != DialogResult.OK || page.SelectedPath is null || string.IsNullOrWhiteSpace(page.SelectedPath))
        //    return;

        //string path = page.SelectedPath;
        //if (!Directory.Exists(path) && !File.Exists(path))
        //    return;

        //result = await this.MessageBox("然后您可以粘贴路径并前往查找您的应用程序", "复制可能所在的路径？", MessageBoxButtons.YesNo);
        //if (result != DialogResult.Yes)
        //    return;

        //ClipboardHelper.Copy(path);
        //StorageFile? file = await FileOpenPickerHelper
        //    .OpenFileForWindow(App.Current.MainWindow)
        //    .AddFileTypeFilter(".exe")
        //    .PickSingleAsync();
        //if (file is null || string.IsNullOrWhiteSpace(file.Path))
        //    return;

        //path = file.Path;
        //string name = Path.GetFileName(path);
        //if (!viewModel.AddProgram(name, path))
        //    await ShowFileNameExistedMessageBox(name);

        InstalledTraditionalProgramListPage page = new();
        DialogResult result;
        result = await this.MessageBox(page, "已安装的程序", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || page.SelectedItem is null || string.IsNullOrWhiteSpace(page.SelectedItem.PossiblePath))
            return;

        string path = page.SelectedItem.PossiblePath;
        if (!Directory.Exists(path) && !File.Exists(path))
            return;

        result = await this.MessageBox("然后您可以粘贴路径并前往查找您的应用程序", "复制可能所在的路径？", MessageBoxButtons.YesNo);
        if (result != DialogResult.Yes)
            return;

        ClipboardHelper.Copy(path);
        StorageFile? file = await FileOpenPickerHelper
            .OpenFileForWindow(App.Current.MainWindow)
            .AddFileTypeFilter(".exe")
            .PickSingleAsync();
        if (file is null || string.IsNullOrWhiteSpace(file.Path))
            return;

        path = file.Path;
        string name = Path.GetFileName(path);
        if (!viewModel.AddProgram(name, path))
            await ShowFileNameExistedMessageBox(name);
    }

    private async Task ShowFileNameExistedMessageBox(string filename)
        => await this.MessageBox("注册表中的映像劫持是按照文件名而不是文件路径，因此即使不同位置同名文件，也会被映像劫持。", $"已添加过同名文件 {filename}");

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        Navigation.IsPaneOpen = true;

        ProgramListProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
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
