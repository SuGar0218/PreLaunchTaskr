// WPF

using CommunityToolkit.Mvvm.ComponentModel;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;

using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

public partial class PreLaunchTaskViewModel :
    ObservableObject,
    IProgramConfigCategoryViewModel,
    IPreLaunchTaskViewModel<PreLaunchTaskListItem>
{
    public PreLaunchTaskViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
    }

    [ObservableProperty]  // 会生成属性 Tasks
    public ObservableCollection<PreLaunchTaskListItem>? _tasks;

    public void Init()
    {
        Tasks = new();
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
        Tasks = new();
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

    private readonly List<PreLaunchTaskListItem> removed = new();
}
