using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// ��ҳ���յ�������Ϊ�����Ӧ�� ExtraData������Ϊ EnvironmentVariableViewModel
/// </summary>
public sealed partial class EnvironmentVariablePage : Page
{
    public EnvironmentVariablePage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (EnvironmentVariableViewModel) e.Parameter;
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

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
        ListLoadingProgressBar.Visibility = Visibility.Collapsed;
    }
}
