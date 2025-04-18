using PreLaunchTaskr.GUI.WPF.Helpers;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Views;

public partial class EnvironmentVariablePage : Page
{
    public EnvironmentVariablePage(EnvironmentVariableViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
    }

    private readonly EnvironmentVariableViewModel viewModel;

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await viewModel.InitAsync();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        viewModel.AddEnvironmentVariable();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

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
