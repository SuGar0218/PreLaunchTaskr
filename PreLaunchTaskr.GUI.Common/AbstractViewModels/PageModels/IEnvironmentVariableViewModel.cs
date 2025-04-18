namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

public interface IEnvironmentVariableViewModel<TEnvironmentVariableListItem>
{
    public void Init();

    public Task InitAsync();

    public void AddEnvironmentVariable();

    public void RemoveEnvironmentVariable(TEnvironmentVariableListItem item);

    public bool SaveChanges();
}
