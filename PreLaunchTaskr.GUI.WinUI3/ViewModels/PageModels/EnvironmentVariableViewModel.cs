using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public class EnvironmentVariableViewModel : ObservableObject, IProgramConfigCategoryViewModel
{
    public EnvironmentVariableViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
        EnvironmentVariables.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
    }

    public ObservableCollection<EnvironmentVariableListItem> EnvironmentVariables { get; } = [];

    public void Init()
    {
        EnvironmentVariables.Clear();
        removed.Clear();
        foreach (EnvironmentVariable variable in App.Current.Configurator.ListEnvironmentVariablesForProgram(programListItem.Id))
        {
            EnvironmentVariables.Add(new EnvironmentVariableListItem(variable));
        }
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(EnvironmentVariables.Clear);
            foreach (EnvironmentVariable variable in App.Current.Configurator.ListEnvironmentVariablesForProgram(programListItem.Id))
            {
                dispatcherQueue.TryEnqueue(() => EnvironmentVariables.Add(new EnvironmentVariableListItem(variable)));
            }
        });
    }

    public void AddEnvironmentVariable()
    {
        EnvironmentVariables.Add(new EnvironmentVariableListItem(new EnvironmentVariable(programListItem.ProgramInfo, string.Empty, string.Empty, false)));
    }

    public void RemoveEnvironmentVariable(EnvironmentVariableListItem item)
    {
        EnvironmentVariables.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
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

    public bool IsListEmpty => EnvironmentVariables.Count == 0;

    private readonly ProgramListItem programListItem;

    private readonly List<EnvironmentVariableListItem> removed = [];
}
