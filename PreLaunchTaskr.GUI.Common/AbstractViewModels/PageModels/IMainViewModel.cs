using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

using System.Collections.ObjectModel;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

public interface IMainViewModel<TProgramListItem, TIcon>
    where TProgramListItem : IProgramListItem<TIcon>
{
    public ObservableCollection<TProgramListItem> Programs { get; }

    public void Init();

    public Task InitAsync();

    public bool AddProgram(string name, string path);

    public bool RemoveProgram(TProgramListItem item);

    public bool EnableProgram(int id);

    public bool DisableProgram(int id);
}
