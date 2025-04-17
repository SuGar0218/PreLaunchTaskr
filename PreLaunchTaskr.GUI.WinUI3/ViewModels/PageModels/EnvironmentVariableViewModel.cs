using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class EnvironmentVariableViewModel : ObservableObject, IProgramConfigCategoryViewModel, IEnvironmentVariableViewModel<DispatcherQueue, EnvironmentVariableListItem>
{
    public EnvironmentVariableViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
    }

    [ObservableProperty]
    public partial ObservableCollection<EnvironmentVariableListItem>? EnvironmentVariables { get; private set; }

    public void Init()
    {
        //EnvironmentVariables.Clear();
        EnvironmentVariables = [];
        EnvironmentVariables.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        foreach (EnvironmentVariable variable in App.Current.Configurator.ListEnvironmentVariablesForProgram(programListItem.Id))
        {
            EnvironmentVariables.Add(new EnvironmentVariableListItem(variable));
        }
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        EnvironmentVariables = [];
        EnvironmentVariables.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        await Task.Run(() =>
        {
            //dispatcherQueue.TryEnqueue(EnvironmentVariables.Clear);
            IList<EnvironmentVariable> environmentVariables = App.Current.Configurator.ListEnvironmentVariablesForProgram(programListItem.Id);
            dispatcherQueue.TryEnqueue(() =>
            {
                foreach (EnvironmentVariable environmentVariable in environmentVariables)
                {
                    EnvironmentVariables.Add(new EnvironmentVariableListItem(environmentVariable));
                }
            });
        });
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public void AddEnvironmentVariable()
    {
        EnvironmentVariables!.Add(new EnvironmentVariableListItem(new EnvironmentVariable(programListItem.ProgramInfo, string.Empty, string.Empty, false)));
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public void RemoveEnvironmentVariable(EnvironmentVariableListItem item)
    {
        EnvironmentVariables!.Remove(item);
        removed.Add(item);
        //OnPropertyChanged(nameof(IsListEmpty));
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

    public bool IsListEmpty => EnvironmentVariables.Count == 0;

    private readonly ProgramListItem programListItem;

    private readonly List<EnvironmentVariableListItem> removed = [];
}
