using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class PreLaunchTaskViewModel :
    ObservableObject,
    IProgramConfigCategoryViewModel,
    IPreLaunchTaskViewModel<PreLaunchTaskListItem>
{
    public PreLaunchTaskViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListEmpty))]
    public partial ObservableCollection<PreLaunchTaskListItem>? Tasks { get; private set; }

    public void Init()
    {
        Tasks = [];
        Tasks.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<PreLaunchTask> preLaunchTasks = App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id);
        foreach (PreLaunchTask preLaunchTask in preLaunchTasks)
        {
            Tasks.Add(new PreLaunchTaskListItem(preLaunchTask));
        }
    }

    public async Task InitAsync()
    {
        Tasks = [];
        Tasks.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<PreLaunchTask> preLaunchTasks = await Task.Run(() => App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id));
        foreach (PreLaunchTask preLaunchTask in preLaunchTasks)
        {
            Tasks.Add(new PreLaunchTaskListItem(preLaunchTask));
        }
    }

    public void AddTask()
    {
        Tasks!.Add(new PreLaunchTaskListItem(new PreLaunchTask(programListItem.ProgramInfo, string.Empty, false, false, false)));
    }

    public void RemoveTask(PreLaunchTaskListItem item)
    {
        Tasks!.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
        if (Tasks is null)
            return true;

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

    public bool IsListEmpty => Tasks is null || Tasks.Count == 0;

    private readonly ProgramListItem programListItem;

    private readonly List<PreLaunchTaskListItem> removed = [];
}
