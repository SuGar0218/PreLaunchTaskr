using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels;

public partial class InstalledPackagedProgramListViewModel : ObservableObject
{
    public void Init()
    {
        if (loaded)
            return;

        loaded = true;
        foreach (Package package in new PackageManager().FindPackagesForUser(WindowsIdentity.GetCurrent().User!.Value))
        {
            programs.Add(new InstalledPackagedProgramListItem(package));
        }
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        if (loaded)
            return;

        loaded = true;
        await Task.Run(() =>
        {
            foreach (Package package in new PackageManager().FindPackagesForUser(WindowsIdentity.GetCurrent().User!.Value))
            {
                dispatcherQueue.TryEnqueue(() => programs.Add(new InstalledPackagedProgramListItem(package)));
            }
        });
    }

    public async Task<int> SearchProgramAsync(string name, DispatcherQueue dispatcherQueue)
    {
        dispatcherQueue.TryEnqueue(SearchedPrograms.Clear);
        await Task.Run(() =>
        {
            foreach (var program in programs)
            {
                string[] words = program.Name.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].StartsWith(name) || words[i].StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        dispatcherQueue.TryEnqueue(() => SearchedPrograms.Add(program));
                        break;
                    }
                }
            }
        });
        return SearchedPrograms.Count;
    }

    public ObservableCollection<InstalledPackagedProgramListItem> Programs => programs;

    public ObservableCollection<InstalledPackagedProgramListItem> SearchedPrograms { get; } = [];

    [ObservableProperty]
    public partial InstalledPackagedProgramListItem? SelectedItem { get; set; }

    private ObservableCollection<InstalledPackagedProgramListItem> programs = [];

    private bool loaded;  // 初始为 false
}
