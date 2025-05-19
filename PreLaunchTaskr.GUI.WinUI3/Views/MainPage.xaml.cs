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
/// ���ܵ�������������Ϊ��MainViewModel
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
    /// ���ܵ�������������Ϊ��MainViewModel
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
            new Win32FilePickerFilter("Ӧ�ó���", "*.exe"),
            new Win32FilePickerFilter("��ݷ�ʽ", "*.exe")],
            "shell:Desktop",
            dereferenceLink: true,
            App.Current.MainWindow.hWnd);

        if (string.IsNullOrWhiteSpace(path))
            return;

        string filename = Path.GetFileName(path);
        if (! viewModel!.AddProgram(filename, path))
        {
            await ShowFileNameExistedMessageBox(filename);
        }
    }

    private async void InputProgramPath()
    {
        InputTextPage page = new();
        DialogResult result = await this.MessageBox(page, "��������λ��", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || string.IsNullOrWhiteSpace(page.Text))
            return;

        string path = StringHelper.TrimQuotes(page.Text.Trim());

        if (!File.Exists(path))
        {
            await this.MessageBox(path, "û������ļ�");
            return;
        }

        string name = Path.GetFileName(path);

        if (! viewModel!.AddProgram(name, path))
            await ShowFileNameExistedMessageBox(name);
    }

    private async void SelectInstalledProgram()
    {
        // �ݲ�֧���̵�Ӧ�ã���Ϊ�̵�Ӧ������·����Ȩ�����ļ������޷������������ӡ�
        //InstalledProgramListPage page = new();
        //DialogResult result;
        //result = await this.MessageBox(page, "�Ѱ�װ�ĳ���", MessageBoxButtons.OKCancel);
        //if (result != DialogResult.OK || page.SelectedPath is null || string.IsNullOrWhiteSpace(page.SelectedPath))
        //    return;

        //string path = page.SelectedPath;
        //if (!Directory.Exists(path) && !File.Exists(path))
        //    return;

        //result = await this.MessageBox("Ȼ��������ճ��·����ǰ����������Ӧ�ó���", "���ƿ������ڵ�·����", MessageBoxButtons.YesNo);
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
        result = await this.MessageBox(page, "�Ѱ�װ�ĳ���", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || page.SelectedItem is null || string.IsNullOrWhiteSpace(page.SelectedItem.PossiblePath))
            return;

        string directory = page.SelectedItem.PossiblePath;
        if (!Directory.Exists(directory) && !File.Exists(directory))
            return;

        string? path = await Win32FilePicker.PickFileAsync([
            new Win32FilePickerFilter("Ӧ�ó���", "*.exe"),
            new Win32FilePickerFilter("��ݷ�ʽ", "*.exe")],
            directory,
            dereferenceLink: true,
            App.Current.MainWindow.hWnd);

        if (string.IsNullOrWhiteSpace(path))
            return;

        string filename = Path.GetFileName(path);
        if (! viewModel!.AddProgram(filename, path))
        {
            await ShowFileNameExistedMessageBox(filename);
        }
    }

    private async Task ShowFileNameExistedMessageBox(string filename)
        => await this.MessageBox("ע����е�ӳ��ٳ��ǰ����ļ����������ļ�·������˼�ʹ��ͬλ��ͬ���ļ���Ҳ�ᱻӳ��ٳ֡�", $"����ӹ�ͬ���ļ� {filename}");

    private async Task ShowFileNameExistedMessageBox(IEnumerable<string> filename)
        => await this.MessageBox($"ע����е�ӳ��ٳ��ǰ����ļ����������ļ�·������˼�ʹ��ͬλ��ͬ���ļ���Ҳ�ᱻӳ��ٳ֡�\n�����ļ�������δ����ӣ�\n{new StringBuilder().AppendJoin('\n', filename)}", $"����ӹ�ͬ���ļ� ");

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
        //AddProgramButton.MinWidth = Navigation.CompactPaneLength - 8;  // �۵�����ർ���˵���ȼ����ߵ� Margin
        //MoreButton.MinWidth = Navigation.CompactPaneLength - 8;  // �۵�����ർ���˵���ȼ����ߵ� Margin
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
                if (Path.GetExtension(item.Name) == ".exe")
                {
                    if (! viewModel!.AddProgram(item.Name, item.Path))
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
                await this.MessageBox($"�����ļ����ļ����Ͳ�֧�ֶ�δ����ӣ�\n{new StringBuilder().AppendJoin('\n', unsupportedFileNames)}", "ֻ����� exe ���͵��ļ�");
            }
        }
    }

    private void Navigation_DragOver(object _, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void ShowDragToAddGuide()
    {
        await this.MessageBox("�ȹرմ���ʾ��Ȼ�������Ҫ��ӵĳ���� exe �ļ���ק�������ɡ�", "��ק�����");
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
            "���ڴ�Ӧ��",
            new SymbolIconSource { Symbol = Symbol.Emoji2 },
            typeof(AboutPage));

        App.Current.MultiTab.TryAddUniqueTabStripItem(
            newTabItem,
            (one, other) => one.PageType == other.PageType);
    }

    private void GoToSettingsPage()
    {
        TabStripItem newTabItem = new(
            "����",
            new SymbolIconSource { Symbol = Symbol.Setting },
            typeof(SettingsPage));

        App.Current.MultiTab.TryAddUniqueTabStripItem(
            newTabItem,
            (one, other) => one.PageType == other.PageType);
    }

    private bool isMiddleButtonPressed;  // ����ʶ������м��㰴������м�����б������±�ǩ�д򿪡�

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
    /// �������û�����������ڰ󶨵����ݱ仯�����ᴥ�� Toggled �¼����ڴ��� isUserToggled ���ж��Ƿ�Ϊ�û������ɵ��л���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (!isUserToggled)
            return;

        isUserToggled = false;

        ProgramListItem? item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        if (item is null)  // ҳ���״μ���ʱ��ȡ���� DataContext �� null
            return;

        ToggleSwitch toggle = (ToggleSwitch) sender;
        bool backup = item.Enabled;
        item.Enabled = toggle.IsOn;
        if (! await Task.Run(item.SaveChanges))
        {
            item.Enabled = backup;
        }
    }

    private bool isUserToggled = false;  // �ж� ToggleSwitch �� Toggled �¼������Ƿ���Ϊ�û�����������ݷ����仯Ҳ�ᴥ�� Toggled �¼���

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
}
