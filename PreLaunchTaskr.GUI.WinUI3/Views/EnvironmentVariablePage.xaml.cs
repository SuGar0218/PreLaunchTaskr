using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 此页接收导航参数为自身对应的 ExtraData，类型为 EnvironmentVariableViewModel
/// </summary>
public sealed partial class EnvironmentVariablePage : Page
{
    public EnvironmentVariablePage()
    {
        InitializeComponent();
    }

    private EnvironmentVariableViewModel viewModel = null!;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (EnvironmentVariableViewModel) e.Parameter;
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

    private void DeleteEnvironmentVariable(object sender, RoutedEventArgs e)
    {
        EnvironmentVariableListItem item = DataContextHelper.GetDataContext<EnvironmentVariableListItem>(sender)!;
        viewModel.RemoveEnvironmentVariable(item);
    }

    private void CopyName(object sender, RoutedEventArgs e)
    {
        EnvironmentVariableListItem item = DataContextHelper.GetDataContext<EnvironmentVariableListItem>(sender)!;
        ClipboardHelper.Copy(item.Key);
    }

    private void CopyValue(object sender, RoutedEventArgs e)
    {
        EnvironmentVariableListItem item = DataContextHelper.GetDataContext<EnvironmentVariableListItem>(sender)!;
        ClipboardHelper.Copy(item.Value);
    }
}
