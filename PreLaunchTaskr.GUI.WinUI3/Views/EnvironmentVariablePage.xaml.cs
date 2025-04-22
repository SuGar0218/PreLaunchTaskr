using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 此页接收导航参数为自身对应的 PageViewModel，类型为 EnvironmentVariableViewModel
/// </summary>
public sealed partial class EnvironmentVariablePage : Page
{
    public EnvironmentVariablePage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (EnvironmentVariableViewModel) e.Parameter;
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
        ListLoadingProgressBar.Visibility = Visibility.Collapsed;
        base.OnNavigatedTo(e);
    }

    private void ConfirmDeleteEnvironmentVariable_Click(object sender, RoutedEventArgs e)
    {
        EnvironmentVariableListItem item = DataContextHelper.GetDataContext<EnvironmentVariableListItem>(sender);
        viewModel.RemoveEnvironmentVariable(item);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private EnvironmentVariableViewModel viewModel = null!;

    private void CopyName_Click(object sender, RoutedEventArgs e)
    {
        EnvironmentVariableListItem item = DataContextHelper.GetDataContext<EnvironmentVariableListItem>(sender);
        ClipboardHelper.Copy(item.Key);
    }

    private void CopyValue_Click(object sender, RoutedEventArgs e)
    {
        EnvironmentVariableListItem item = DataContextHelper.GetDataContext<EnvironmentVariableListItem>(sender);
        ClipboardHelper.Copy(item.Value);
    }
}
