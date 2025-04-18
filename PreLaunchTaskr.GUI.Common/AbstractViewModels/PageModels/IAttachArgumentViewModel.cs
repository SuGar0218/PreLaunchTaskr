using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

public interface IAttachArgumentViewModel<TAttachedArgumentListItem>
    where TAttachedArgumentListItem : IAttachedArgumentListItem
{
    public void Init();

    public Task InitAsync();

    public void AddArgument();

    public void RemoveArgument(TAttachedArgumentListItem item);

    public bool SaveChanges();
}
