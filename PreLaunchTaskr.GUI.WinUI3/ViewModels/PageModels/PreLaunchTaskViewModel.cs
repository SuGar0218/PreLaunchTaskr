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

public class PreLaunchTaskViewModel : ObservableObject, IProgramConfigCategoryViewModel
{
    public PreLaunchTaskViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
        Tasks.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
    }

    public ObservableCollection<PreLaunchTaskListItem> Tasks { get; } = [];

    public void Init()
    {
        Tasks.Clear();
        removed.Clear();
        foreach (PreLaunchTask task in App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id))
        {
            Tasks.Add(new PreLaunchTaskListItem(task));
        }
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(Tasks.Clear);
            foreach (PreLaunchTask task in App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id))
            {
                dispatcherQueue.TryEnqueue(() => Tasks.Add(new PreLaunchTaskListItem(task)));
            }
        });
    }

    public void AddTask()
    {
        Tasks.Add(new PreLaunchTaskListItem(new PreLaunchTask(programListItem.ProgramInfo, string.Empty, false, false, false)));
    }

    public void RemoveTask(PreLaunchTaskListItem item)
    {
        Tasks.Remove(item);
    }

    public bool SaveChanges()
    {
        foreach (PreLaunchTaskListItem item in Tasks)
        {
            item.SaveChanges();
        }
        foreach (PreLaunchTaskListItem item in removed)
        {
            item.Remove();
        }
        removed.Clear();
        return true;
    }

    public bool IsListEmpty => Tasks.Count == 0;

    private readonly ProgramListItem programListItem;

    private readonly List<PreLaunchTaskListItem> removed = [];
}
