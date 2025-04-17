using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class PreLaunchTaskViewModel : ObservableObject, IProgramConfigCategoryViewModel, IPreLaunchTaskViewModel<DispatcherQueue, PreLaunchTaskListItem>
{
    public PreLaunchTaskViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
        Tasks = null!;
    }

    [ObservableProperty]
    public partial ObservableCollection<PreLaunchTaskListItem> Tasks { get; private set; }

    public void Init()
    {
        //Tasks.Clear();
        Tasks = [];
        Tasks.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        foreach (PreLaunchTask task in App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id))
        {
            Tasks.Add(new PreLaunchTaskListItem(task));
        }
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        Tasks = [];
        Tasks.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(Tasks.Clear);
            //foreach (PreLaunchTask task in App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id))
            //{
            //    dispatcherQueue.TryEnqueue(() => Tasks.Add(new PreLaunchTaskListItem(task)));
            //}
            IList<PreLaunchTask> preLaunchTasks = App.Current.Configurator.ListPreLaunchTaskForProgram(programListItem.Id);
            dispatcherQueue.TryEnqueue(() =>
            {
                foreach (PreLaunchTask preLaunchTask in preLaunchTasks)
                {
                    Tasks.Add(new PreLaunchTaskListItem(preLaunchTask));
                }
            });
        });
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public void AddTask()
    {
        Tasks.Add(new PreLaunchTaskListItem(new PreLaunchTask(programListItem.ProgramInfo, string.Empty, false, false, false)));
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public void RemoveTask(PreLaunchTaskListItem item)
    {
        Tasks.Remove(item);
        removed.Add(item);
        //OnPropertyChanged(nameof(IsListEmpty));
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
