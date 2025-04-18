namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IPreLaunchTaskListItem : ISaveableItem, IRemoveableItem
{
    public string Path { get; set; }

    public bool Enabled { get; set; }

    public bool AcceptProgramArgs { get; set; }

    public bool IncludeAttachedArgs { get; set; }
}
