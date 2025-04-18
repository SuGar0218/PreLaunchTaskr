﻿using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.Core.Extensions;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels;

public partial class InstalledTraditionalProgramListViewModel : ObservableObject
{
    public void Init()
    {
        if (loaded)
            return;

        loaded = true;
        List<WindowsHelper.ProgramUninstallInfo> programUninstallInfos = WindowsHelper.ListProgramUninstallInfo();
        foreach (WindowsHelper.ProgramUninstallInfo programUninstallInfo in programUninstallInfos)
        {
            programs.Add(new InstalledTraditionalProgramListItem(programUninstallInfo));
        }
    }

    public async Task InitAsync()
    {
        if (loaded)
            return;

        loaded = true;
        List<WindowsHelper.ProgramUninstallInfo> programUninstallInfos = await Task.Run(() => WindowsHelper.ListProgramUninstallInfo());
        foreach (WindowsHelper.ProgramUninstallInfo programUninstallInfo in programUninstallInfos)
        {
            InstalledTraditionalProgramListItem item = new InstalledTraditionalProgramListItem(programUninstallInfo);
            if (!string.IsNullOrWhiteSpace(item.PossiblePath))
            {
                programs.Add(item);
            }
        }
    }

    public async Task<int> SearchProgramAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 0;

        List<InstalledTraditionalProgramListItem> result = [];
        await Task.Run(() =>
        {
            foreach (var program in programs)
            {
                string[] words = program.Name.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].StartsWith(name) || words[i].StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add(program);
                        break;
                    }
                }
            }
        });
        SearchedPrograms = new ObservableCollection<InstalledTraditionalProgramListItem>(result);
        return SearchedPrograms.Count;
    }

    public ObservableCollection<InstalledTraditionalProgramListItem> Programs => programs;

    [ObservableProperty]
    public partial ObservableCollection<InstalledTraditionalProgramListItem> SearchedPrograms { get; private set; }

    [ObservableProperty]
    public partial InstalledTraditionalProgramListItem? SelectedItem { get; set; }

    private static readonly ObservableCollection<InstalledTraditionalProgramListItem> programs = [];

    private static bool loaded;  // 初始为 false
}
