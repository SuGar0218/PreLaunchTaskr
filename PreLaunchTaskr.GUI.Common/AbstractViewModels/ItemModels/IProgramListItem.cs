using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels.Base;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IProgramListItem<TImage> : ISaveableItem, IRemoveableItem
{
    public int Id { get; }
    public string Name { get; }
    public string Path { get; }
    public TImage? Icon { get; }
    public bool Enabled { get; set; }
    public ProgramInfo ProgramInfo { get; }
}
