namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IEnvironmentVariableListItem : ISaveableItem, IRemoveableItem
{
    public string Key { get; set; }

    public string Value { get; set; }

    public bool Enabled { get; set; }
}
