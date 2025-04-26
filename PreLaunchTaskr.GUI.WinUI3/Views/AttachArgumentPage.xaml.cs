using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 此页接收导航参数为自身对应的 ExtraData，类型为 AttachArgumentViewModel
/// </summary>
public sealed partial class AttachArgumentPage : Page
{
    public AttachArgumentPage()
    {
        InitializeComponent();
    }

    private AttachArgumentViewModel viewModel = null!;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (AttachArgumentViewModel) e.Parameter;
        base.OnNavigatedTo(e);
    }

    private void ConfirmDeleteArgument_Click(object sender, RoutedEventArgs e)
    {
        AttachedArgumentListItem item = DataContextHelper.GetDataContext<AttachedArgumentListItem>(sender);
        viewModel.RemoveArgument(item);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private void CopyArgument_Click(object sender, RoutedEventArgs e)
    {
        AttachedArgumentListItem item = DataContextHelper.GetDataContext<AttachedArgumentListItem>(sender);
        ClipboardHelper.Copy(item.Argument);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
        //viewModel.Init();
        ListLoadingProgressBar.Visibility = Visibility.Collapsed;
    }
}

/*
 * 退出页面时保存数据，进入页面时加载数据，要注意页面导航的事件顺序。
 * 从另一页导航过来时，在 Windows App SDK 事件按以下顺序发生：
 * 这一页.NavigatedTo
 * 上一页.Unloaded
 * 这一页.Loaded
 */
