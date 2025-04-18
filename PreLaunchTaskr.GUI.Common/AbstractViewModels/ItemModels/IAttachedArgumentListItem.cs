namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IAttachedArgumentListItem : ISaveableItem, IRemoveableItem
{
    public string Argument { get; set; }

    public bool Enabled { get; set; }
}
