﻿using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class EnvironmentVariableViewModel :
    ObservableObject,
    IProgramConfigCategoryViewModel,
    IEnvironmentVariableViewModel<EnvironmentVariableListItem>
{
    public EnvironmentVariableViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListEmpty))]
    public partial ObservableCollection<EnvironmentVariableListItem>? EnvironmentVariables { get; private set; }

    public void Init()
    {
        EnvironmentVariables = [];
        EnvironmentVariables.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<EnvironmentVariable> environmentVariables = App.Current.Configurator.ListEnvironmentVariablesForProgram(programListItem.Id);
        foreach (EnvironmentVariable environmentVariable in environmentVariables)
        {
            EnvironmentVariables.Add(new EnvironmentVariableListItem(environmentVariable));
        }
    }

    public async Task InitAsync()
    {
        EnvironmentVariables = [];
        EnvironmentVariables.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<EnvironmentVariable> environmentVariables = await Task.Run(() => App.Current.Configurator.ListEnvironmentVariablesForProgram(programListItem.Id));
        foreach (EnvironmentVariable environmentVariable in environmentVariables)
        {
            EnvironmentVariables.Add(new EnvironmentVariableListItem(environmentVariable));
        }
    }

    public void AddEnvironmentVariable()
    {
        EnvironmentVariables!.Add(new EnvironmentVariableListItem(new EnvironmentVariable(programListItem.ProgramInfo, string.Empty, string.Empty, false)));
    }

    public void RemoveEnvironmentVariable(EnvironmentVariableListItem item)
    {
        EnvironmentVariables!.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
        if (EnvironmentVariables is null)
            return true;

        foreach (EnvironmentVariableListItem item in EnvironmentVariables)
        {
            item.SaveChanges();
        }
        foreach (EnvironmentVariableListItem item in removed)
        {
            item.Remove();
        }
        removed.Clear();
        return true;
    }

    public bool IsListEmpty => EnvironmentVariables is null || EnvironmentVariables.Count == 0;

    private readonly ProgramListItem programListItem;

    private readonly List<EnvironmentVariableListItem> removed = [];
}
