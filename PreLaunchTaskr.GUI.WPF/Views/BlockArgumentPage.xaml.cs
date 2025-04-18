using PreLaunchTaskr.GUI.WPF.Helpers;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Views;

/// <summary>
/// BlockArgumentPage.xaml 的交互逻辑
/// </summary>
public partial class BlockArgumentPage : Page
{
    public BlockArgumentPage(BlockArgumentViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
    }

    private readonly BlockArgumentViewModel viewModel;

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
        viewModel.AddArgument();
    }

    private void CopyArgument_Click(object sender, RoutedEventArgs e)
    {
        BlockedArgumentListItem item = DataContextHelper.GetDataContext<BlockedArgumentListItem>(sender);
        ClipboardHelper.Copy(item.Argument);
    }
}
