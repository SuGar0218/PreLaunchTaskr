using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.ViewModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class InstalledProgramListPage : Page
{
    public InstalledProgramListPage()
    {
        InitializeComponent();
    }

    private InstalledTraditionalProgramListViewModel traditionalProgramListViewModel = new();
    private InstalledPackagedProgramListViewModel packagedProgramListViewModel = new();

    private NavigationViewItem? selectedNavigationItem;

    public string? SelectedPath
    {
        get
        {
            if (selectedNavigationItem is null)
                return null;

            if (selectedNavigationItem.DataContext == traditionalProgramListViewModel)
            {
                if (traditionalProgramListViewModel.SelectedItem is null)
                    return null;

                return traditionalProgramListViewModel.SelectedItem.PossiblePath;
            }

            if (selectedNavigationItem.DataContext == packagedProgramListViewModel)
            {
                if (packagedProgramListViewModel.SelectedItem is null)
                    return null;

                return packagedProgramListViewModel.SelectedItem.InstallPath;
            }

            throw new ArgumentException();
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is null)
            return;

        NavigationViewItem selectedItem = (NavigationViewItem) args.SelectedItem;
        if (selectedItem.DataContext == traditionalProgramListViewModel)
        {
            ContentFrame.Navigate(typeof(InstalledTraditionalProgramListPage), traditionalProgramListViewModel, args.RecommendedNavigationTransitionInfo);
        }
        else if (selectedItem.DataContext == packagedProgramListViewModel)
        {
            ContentFrame.Navigate(typeof(InstalledPackagedProgramListPage), packagedProgramListViewModel, args.RecommendedNavigationTransitionInfo);
        }
    }
}
