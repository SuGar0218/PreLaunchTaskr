namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IBlockedArgumentListItem : ISaveableItem, IRemoveableItem
{
    public string Argument { get; set; }

    public bool Enabled { get; set; }

    public bool IsRegex { get; set; }
}
