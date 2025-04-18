using PreLaunchTaskr.GUI.WPF.Helpers;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Views;

public partial class AttachArgumentPage : Page
{
    public AttachArgumentPage(AttachArgumentViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
    }

    private AttachArgumentViewModel viewModel;

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await viewModel.InitAsync();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        viewModel.AddArgument();
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
