using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

public interface IPreLaunchTaskViewModel<TPreLaunchTaskListItem>
    where TPreLaunchTaskListItem : IPreLaunchTaskListItem
{
    public void Init();

    public Task InitAsync();

    public void AddTask();

    public void RemoveTask(TPreLaunchTaskListItem item);

    public bool SaveChanges();
}
