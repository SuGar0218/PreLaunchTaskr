using Microsoft.Win32;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WPF.Helpers;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Views;

public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private readonly MainViewModel viewModel = new();

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        TitleBarArea.Visibility = App.Current.MainWindow.ExtendsContentIntoTitleBar ? Visibility.Visible : Visibility.Collapsed;
        ContentFrame.Navigate(ProgramUnselectedPage.StaticPage);
        await viewModel.InitAsync();
    }

    private void ProgramList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0)
        {
            ContentFrame.Navigate(ProgramUnselectedPage.StaticPage);
            return;
        }

        ProgramListItem item = (ProgramListItem) e.AddedItems[0]!;
        ProgramConfigPage page = new(new ProgramConfigViewModel(item));
        ContentFrame.Navigate(page);
    }

    private void SelectProgramFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            Filter = "应用程序|*.exe"
        };
        dialog.ShowDialog();
        if (string.IsNullOrWhiteSpace(dialog.FileName))
            return;

        if (!viewModel.AddProgram(dialog.SafeFileName, dialog.FileName))
        {
            MessageBox.Show("注册表中的映像劫持是按照文件名而不是文件路径，因此即使不同位置同名文件，也会被映像劫持。", $"已添加过同名文件 {dialog.FileName}");
        }
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
