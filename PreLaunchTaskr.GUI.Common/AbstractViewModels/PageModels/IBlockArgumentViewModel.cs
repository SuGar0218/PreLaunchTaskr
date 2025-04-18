using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

public interface IBlockArgumentViewModel<TBlockedArgumentListItem>
    where TBlockedArgumentListItem : IBlockedArgumentListItem
{
    public void Init();

    public Task InitAsync();

    public void AddArgument();

    public void RemoveArgument(TBlockedArgumentListItem item);

    public bool SaveChanges();
}
