using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 此页接收导航参数为自身对应的 ExtraData，类型为 PreLaunchTaskViewModel
/// </summary>
public sealed partial class PreLaunchTaskPage : Page
{
    public PreLaunchTaskPage()
    {
        InitializeComponent();
    }

    private PreLaunchTaskViewModel viewModel = null!;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (PreLaunchTaskViewModel) e.Parameter;
        base.OnNavigatedTo(e);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
        ListLoadingProgressBar.Visibility = Visibility.Collapsed;
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private void DeleteTask(object sender, object _)
    {
        PreLaunchTaskListItem item = DataContextHelper.GetDataContext<PreLaunchTaskListItem>(sender)!;
        viewModel.RemoveTask(item);
    }

    private void CopyPath(object sender, object _)
    {
        PreLaunchTaskListItem item = DataContextHelper.GetDataContext<PreLaunchTaskListItem>(sender)!;
        ClipboardHelper.Copy(item.Path);
    }
}
