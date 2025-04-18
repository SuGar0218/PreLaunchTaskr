using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 此页接收导航参数为自身对应的 PageViewModel，类型为 AttachArgumentViewModel
/// </summary>
public sealed partial class AttachArgumentPage : Page
{
    public AttachArgumentPage()
    {
        InitializeComponent();
    }

    private AttachArgumentViewModel viewModel = null!;

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (AttachArgumentViewModel) e.Parameter;
        ListLoadingProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
        //viewModel.Init();
        ListLoadingProgressBar.Visibility = Visibility.Collapsed;
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
}
