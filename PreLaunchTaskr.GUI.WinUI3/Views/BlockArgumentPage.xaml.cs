using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// ��ҳ���յ�������Ϊ�����Ӧ�� ExtraData������Ϊ BlockArgumentPage
/// </summary>
public sealed partial class BlockArgumentPage : Page
{
    public BlockArgumentPage()
    {
        InitializeComponent();
    }

    private BlockArgumentViewModel viewModel = null!;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (BlockArgumentViewModel) e.Parameter;
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

    private void DeleteArgument(object sender, RoutedEventArgs e)
    {
        BlockedArgumentListItem item = DataContextHelper.GetDataContext<BlockedArgumentListItem>(sender)!;
        viewModel.RemoveArgument(item);
    }

    private void CopyArgument(object sender, RoutedEventArgs e)
    {
        BlockedArgumentListItem item = DataContextHelper.GetDataContext<BlockedArgumentListItem>(sender)!;
        ClipboardHelper.Copy(item.Argument);
    }
}
