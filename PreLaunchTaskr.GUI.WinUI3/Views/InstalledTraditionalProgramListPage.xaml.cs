using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// 导航参数为 InstalledTraditionalProgramListViewModel
/// </summary>
public sealed partial class InstalledTraditionalProgramListPage : Page
{
    public InstalledTraditionalProgramListPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (InstalledTraditionalProgramListViewModel) e.Parameter;
        base.OnNavigatedTo(e);
    }

    private InstalledTraditionalProgramListViewModel viewModel = new();

    public InstalledTraditionalProgramListItem? SelectedItem => viewModel.SelectedItem;

    private bool loaded;  // 初始为 false

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (!loaded)
        {
            loaded = true;
            await LoadAsync();
        }
    }

    private async Task LoadAsync()
    {
        ProgramListProgressRing.IsActive = true;
        await viewModel.InitAsync();
        ProgramListProgressRing.IsActive = false;
    }

    private void Border_Loaded(object sender, RoutedEventArgs e)
    {
        Border border = (Border) sender;
        border.Shadow = shadow;
        border.Translation += new Vector3(0, 0, 16);
    }

    private static readonly ThemeShadow shadow = new();

    private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        await viewModel.SearchProgramAsync(sender.Text);
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        InstalledTraditionalProgramListItem item = (InstalledTraditionalProgramListItem) args.SelectedItem;
        sender.Text = item.Name;
        viewModel.SelectedItem = item;
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] is null)
            return;

        ListView listView = (ListView) sender;
        listView.ScrollIntoView(e.AddedItems[0]);
    }

    private void GoToPath_Click(object sender, RoutedEventArgs e)
    {
        InstalledTraditionalProgramListItem item = DataContextHelper.GetDataContext<InstalledTraditionalProgramListItem>(sender);
        WindowsHelper.OpenPathInExplorer(item.PossiblePath);
    }

    private void CopyPath_Click(object sender, RoutedEventArgs e)
    {
        InstalledTraditionalProgramListItem item = DataContextHelper.GetDataContext<InstalledTraditionalProgramListItem>(sender);
        ClipboardHelper.Copy(item.PossiblePath);
    }
}