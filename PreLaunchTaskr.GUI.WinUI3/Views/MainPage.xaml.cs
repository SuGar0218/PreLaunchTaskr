using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Extensions;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 接受导航参数的类型为：MainViewModel
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage() : this(null) { }

    public MainPage(MainViewModel? viewModel)
    {
        InitializeComponent();

        Navigation.ExpandedModeThresholdWidth = Math.E * Navigation.OpenPaneLength;
        this.viewModel = viewModel;

        App.Current.MultiTab.TabStripHidden += (o, e) =>
        {
            App.Current.MainWindow.SetTitleBar(TitleBarArea);
            titleBarPassthroughHelper.Passthrough(TitleBarToggleButton);
            //App.Current.MainWindow.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        };
        //App.Current.MultiTab.TabStripShown += (o, e) =>
        //{
        //    App.Current.MainWindow.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;
        //};
    }

    private MainViewModel? viewModel;

    /// <summary>
    /// 接受导航参数的类型为：MainViewModel
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        MainViewModel? newViewModel = (MainViewModel?) e.Parameter;
        if (viewModel is null || (newViewModel is not null && newViewModel != viewModel))
            viewModel = newViewModel;
        base.OnNavigatedTo(e);
    }

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = TitleBarPassthroughHelper.For(App.Current.MainWindow);

    //private void ShowAddProgramMenu(object sender, object _)
    //{
    //    AddProgramMenu.ShowAt((FrameworkElement) sender);
    //}

    private async void SelectProgramFromFile()
    {
        string? path = await Win32FilePicker.PickFileAsync([
            new Win32FilePickerFilter("应用程序", "*.exe"),
            new Win32FilePickerFilter("快捷方式", "*.exe")],
            "shell:Desktop",
            dereferenceLink: true,
            App.Current.MainWindow.hWnd);

        if (string.IsNullOrWhiteSpace(path))
            return;

        string name = FileDescriber.Describe(path);
        if (! viewModel!.AddProgram(name, path))
        {
            await ShowFileNameExistedMessageBox(name);
        }
    }

    private async void InputProgramPath()
    {
        InputTextPage page = new();
        DialogResult result = await this.MessageBox(page, "程序所在位置", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || string.IsNullOrWhiteSpace(page.Text))
            return;

        string path = StringHelper.TrimQuotes(page.Text.Trim());

        if (!File.Exists(path))
        {
            await this.MessageBox(path, "没有这个文件");
            return;
        }

        string name = FileDescriber.Describe(path);

        if (! viewModel!.AddProgram(name, path))
        {
            await ShowFileNameExistedMessageBox(name);
        }
    }

    private async void SelectInstalledProgram()
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

        InstalledTraditionalProgramListPage page = new() { Width = 480 };
        DialogResult result;
        result = await this.MessageBox(page, "已安装的程序", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || page.SelectedItem is null || string.IsNullOrWhiteSpace(page.SelectedItem.PossiblePath))
            return;

        string directory = page.SelectedItem.PossiblePath;
        if (!Directory.Exists(directory) && !File.Exists(directory))
            return;

        string? path = await Win32FilePicker.PickFileAsync([
            new Win32FilePickerFilter("应用程序", "*.exe"),
            new Win32FilePickerFilter("快捷方式", "*.exe")],
            directory,
            dereferenceLink: true,
            App.Current.MainWindow.hWnd);

        if (string.IsNullOrWhiteSpace(path))
            return;

        string name = FileDescriber.Describe(path);

        if (! viewModel!.AddProgram(name, path))
        {
            await ShowFileNameExistedMessageBox(name);
        }
    }

    private async Task ShowFileNameExistedMessageBox(string filename)
        => await this.MessageBox("注册表中的映像劫持是按照文件名而不是文件路径，因此即使不同位置同名文件，也会被映像劫持。", $"已添加过同名文件 {filename}");

    private async Task ShowFileNameExistedMessageBox(IEnumerable<string> filename)
        => await this.MessageBox($"注册表中的映像劫持是按照文件名而不是文件路径，因此即使不同位置同名文件，也会被映像劫持。\n以下文件因重名未被添加：\n{new StringBuilder().AppendJoin('\n', filename)}", $"已添加过同名文件 ");

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //Navigation.IsPaneOpen = true;
        if (!firstLoaded)
            return;

        firstLoaded = false;
        LoadProgramList();
        //App.Current.MainWindow.ExtendsContentIntoTitleBar = true;
        //App.Current.MainWindow.SetTitleBar(TitleBarBorder);
        //titleBarPassthroughHelper.Passthrough(TitleBarToggleButton);
        //AddProgramButton.MinWidth = Navigation.CompactPaneLength - 8;  // 折叠的左侧导航菜单宽度减两边的 Margin
        //MoreButton.MinWidth = Navigation.CompactPaneLength - 8;  // 折叠的左侧导航菜单宽度减两边的 Margin
        // <Thickness x:Key="NavigationViewItemButtonMargin">4,2</Thickness>
        // https://github.com/microsoft/microsoft-ui-xaml/blob/main/src/controls/dev/NavigationView/NavigationView_themeresources.xaml
    }

    private async void LoadProgramList()
    {
        ProgramListProgressBar.Visibility = Visibility.Visible;
        await viewModel!.InitAsync();
        ProgramListProgressBar.Visibility = Visibility.Collapsed;
        ContentFrame.Navigate(typeof(ProgramUnselectedPage));
    }

    private void ToggleNavigationMenuPane()
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

        if (args.IsSettingsSelected)
        {
            return;
        }

        ContentFrame.Navigate(typeof(ProgramConfigPage), args.SelectedItem);
    }

    private void RemoveProgram(object sender, object _)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender)!;
        viewModel!.RemoveProgram(item);
    }

    private void CopyProgramPath(object sender, object _)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender)!;
        ClipboardHelper.Copy(item.Path);
    }

    private void GoToProgramPath(object sender, object _)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender)!;
        WindowsHelper.OpenPathInExplorer(item.Path);
    }

    private async void AddProgramFromDrop(object _, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 0)
                return;

            List<string> duplicatedFileNames = [];
            List<string> unsupportedFileNames = [];
            foreach (IStorageItem item in items)
            {
                string? targetPath;
                string? targetName;

                string extension = Path.GetExtension(item.Name).ToLowerInvariant();
                
                if (extension == ".lnk")
                {
                    try
                    {
                        targetPath = ShortcutResolver.GetPathFromShortcut(item.Path);
                    }
                    catch (Exception)
                    {
                        unsupportedFileNames.Add(item.Path);
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(targetPath))
                    {
                        unsupportedFileNames.Add(item.Path);
                        continue;
                    }
                    targetName = Path.GetFileName(targetPath);
                }
                else if (extension == ".exe")
                {
                    targetPath = item.Path;
                    targetName = item.Name;
                }
                else
                {
                    unsupportedFileNames.Add(item.Path);
                    continue;
                }
                
                if (Path.GetExtension(targetName)?.ToLowerInvariant() == ".exe")
                {
                    if (!viewModel!.AddProgram(FileDescriber.Describe(targetPath), targetPath))
                    {
                        duplicatedFileNames.Add(item.Path);
                    }
                }
                else
                {
                    unsupportedFileNames.Add(item.Path);
                }
            }
            if (duplicatedFileNames.Count > 0)
            {
                await ShowFileNameExistedMessageBox(duplicatedFileNames);
            }
            if (unsupportedFileNames.Count > 0)
            {
                await this.MessageBox($"以下文件因文件类型不支持或快捷方式解析失败而未被添加：\n{new StringBuilder().AppendJoin('\n', unsupportedFileNames)}", "只能添加 exe 和部分 dll 类型的文件");
            }
        }
    }

    private void Navigation_DragOver(object _, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void ShowDragToAddGuide()
    {
        await this.MessageBox("先关闭此提示，然后把你需要添加的程序的 exe 文件拖拽进来即可。", "拖拽以添加");
    }

    //private void ShowMoreOptionsMenu(object sender, object _)
    //{
    //    MoreOptionsMenu.ShowAt((FrameworkElement) sender);
    //}

    private void ConfigInNewTab(object sender, object _)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender)!;

        TabStripItem newTabItem = new(
            item.Name,
            new ImageIconSource { ImageSource = item.Icon },
            typeof(ProgramConfigPage),
            item);

        App.Current.MultiTab.TryAddUniqueTabStripItem(
            newTabItem,
            (one, other) => one.PageType == other.PageType && one.ExtraData == other.ExtraData);
    }

    private static bool firstLoaded = true;

    private void ShowAboutProgramPage()
    {
        TabStripItem newTabItem = new(
            "关于此应用",
            new SymbolIconSource { Symbol = Symbol.Emoji2 },
            typeof(AboutPage));

        App.Current.MultiTab.TryAddUniqueTabStripItem(
            newTabItem,
            (one, other) => one.PageType == other.PageType);
    }

    private void GoToSettingsPage()
    {
        TabStripItem newTabItem = new(
            "设置",
            new SymbolIconSource { Symbol = Symbol.Setting },
            typeof(SettingsPage));

        App.Current.MultiTab.TryAddUniqueTabStripItem(
            newTabItem,
            (one, other) => one.PageType == other.PageType);
    }

    private bool isMiddleButtonPressed;  // 用于识别鼠标中键点按，鼠标中键点击列表项在新标签中打开。

    private void NavigationViewItem_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        PointerPoint pointerPoint = e.GetCurrentPoint((UIElement) sender);
        isMiddleButtonPressed = pointerPoint.Properties.IsMiddleButtonPressed;
    }

    private void NavigationViewItem_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (isMiddleButtonPressed)
        {
            isMiddleButtonPressed = false;
            ConfigInNewTab(sender, null!);
        }
    }

    private async void EnableAll(object sender, object e)
    {
        await viewModel!.EnableAllPrograms(true);
    }

    private async void DisableAll(object sender, object e)
    {
        await viewModel!.EnableAllPrograms(false);
    }

    /// <summary>
    /// 无论是用户点击还是由于绑定的数据变化，都会触发 Toggled 事件。在此用 isUserToggled 来判断是否为用户点击造成的切换。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (!isUserToggled)
            return;

        isUserToggled = false;

        ProgramListItem? item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        if (item is null)  // 页面首次加载时获取到的 DataContext 是 null
            return;

        ToggleSwitch toggle = (ToggleSwitch) sender;
        bool backup = item.Enabled;
        item.Enabled = toggle.IsOn;
        if (! await Task.Run(item.SaveChanges))
        {
            item.Enabled = backup;
        }
    }

    private bool isUserToggled = false;  // 判断 ToggleSwitch 的 Toggled 事件发生是否因为用户点击，绑定数据发生变化也会触发 Toggled 事件。

    private void RemoveSelectedItem(object sender, object e)
    {
        viewModel!.RemoveProgram(viewModel.SelectedItem);
    }

    private async void RemoveAllItems(object sender, object e)
    {
        await viewModel!.RemoveAllPrograms();
    }

    private void ToggleSwitch_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        isUserToggled = true;
    }

    private void ToggleSwitch_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        isUserToggled = true;
    }

    private void ShowContextMenuOnTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement frameworkElement)
        {
            frameworkElement.ContextFlyout?.ShowAt(frameworkElement);
        }
    }

    private void PaneWidthSizer_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
       PaneWidthSizer.Opacity = 1.0;
    }

    private void PaneWidthSizer_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
       PaneWidthSizer.Opacity = 0.0;
    }
}
