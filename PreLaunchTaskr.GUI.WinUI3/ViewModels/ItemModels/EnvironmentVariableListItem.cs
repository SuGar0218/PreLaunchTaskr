using PreLaunchTaskr.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class EnvironmentVariableListItem
{
    public EnvironmentVariableListItem(EnvironmentVariable environmentVariable)
    {
        this.environmentVariable = environmentVariable;
        changed = false;
    }

    public string Key
    {
        get => environmentVariable.Key;
        set
        {
            environmentVariable.Key = value;
            changed = true;
        }
    }

    public string Value
    {
        get => environmentVariable.Value;
        set
        {
            environmentVariable.Value = value;
            changed = true;
        }
    }

    public bool Enabled
    {
        get => environmentVariable.Enabled;
        set
        {
            environmentVariable.Enabled = value;
            changed = true;
        }
    }

    public bool SaveChanges()
    {
        if (string.IsNullOrWhiteSpace(Key))
            return false;

        if (environmentVariable.Id == -1)
            return App.Current.Configurator.AddEnvironmentVariable(environmentVariable) is not null;

        if (!changed)
            return true;

        changed = false;
        return App.Current.Configurator.UpdateEnvironmentVariable(environmentVariable);
    }

    public bool Remove()
    {
        if (environmentVariable.Id == -1)
            return false;

        return App.Current.Configurator.RemoveEnvironmentVariable(environmentVariable.Id);
    }

    private readonly EnvironmentVariable environmentVariable;

    private bool changed;
}
