using PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Views;

public partial class ProgramConfigPage : Page
{
    public ProgramConfigPage(ProgramConfigViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        this.viewModel = viewModel;
    }

    private readonly ProgramConfigViewModel viewModel;

    private void Apply_Click(object sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private void Launch_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(viewModel.ProgramListItem.Path);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LaunchAsAdmin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process process = new();
            process.StartInfo.FileName = viewModel.ProgramListItem.Path;
            process.StartInfo.Verb = "runas";
            process.Start();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
