using PreLaunchTaskr.GUI.WPF.Helpers;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Views;

public partial class PreLaunchTaskPage : Page
{
    public PreLaunchTaskPage(PreLaunchTaskViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
    }

    private readonly PreLaunchTaskViewModel viewModel;

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await viewModel.InitAsync();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        viewModel.AddTask();
    }

    private void CopyPath_Click(object sender, RoutedEventArgs e)
    {
        PreLaunchTaskListItem item = DataContextHelper.GetDataContext<PreLaunchTaskListItem>(sender);
        ClipboardHelper.Copy(item.Path);
    }
}
