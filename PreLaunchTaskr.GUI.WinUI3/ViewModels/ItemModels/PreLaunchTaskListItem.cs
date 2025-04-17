using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class PreLaunchTaskListItem : IPreLaunchTaskListItem
{
    public PreLaunchTaskListItem(PreLaunchTask preLaunchTask)
    {
        this.preLaunchTask = preLaunchTask;
        changed = false;
    }

    public string Path
    {
        get => preLaunchTask.TaskPath;
        set
        {
            preLaunchTask.TaskPath = value;
            changed = true;
        }
    }

    public bool Enabled
    {
        get => preLaunchTask.Enabled;
        set
        {
            preLaunchTask.Enabled = value;
            changed = true;
        }
    }

    public bool AcceptProgramArgs
    {
        get => preLaunchTask.AcceptProgramArgs;
        set
        {
            preLaunchTask.AcceptProgramArgs = value;
            changed = true;
        }
    }

    public bool IncludeAttachedArgs
    {
        get => preLaunchTask.IncludeAttachedArgs;
        set
        {
            preLaunchTask.IncludeAttachedArgs = value;
            changed = true;
        }
    }

    public bool SaveChanges()
    {
        if (string.IsNullOrWhiteSpace(Path))
            return false;

        if (preLaunchTask.Id == -1)
            return App.Current.Configurator.AddPreLaunchTask(preLaunchTask) is not null;

        if (!changed)
            return true;

        changed = false;
        return App.Current.Configurator.UpdatePreLaunchTask(preLaunchTask);
    }

    public bool Remove()
    {
        if (preLaunchTask.Id == -1)
            return false;

        return App.Current.Configurator.RemoveAttachedArgument(preLaunchTask.Id);
    }

    private readonly PreLaunchTask preLaunchTask;

    private bool changed;
}
