using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 此页接收导航参数为自身对应的 PageViewModel，类型为 PreLaunchTaskViewModel
/// </summary>
public sealed partial class PreLaunchTaskPage : Page
{
    public PreLaunchTaskPage()
    {
        InitializeComponent();
    }

    private PreLaunchTaskViewModel viewModel = null!;

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (PreLaunchTaskViewModel) e.Parameter;
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
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

    private void CopyPath_Click(object sender, RoutedEventArgs e)
    {
        PreLaunchTaskListItem item = DataContextHelper.GetDataContext<PreLaunchTaskListItem>(sender);
        ClipboardHelper.Copy(item.Path);
    }
}
