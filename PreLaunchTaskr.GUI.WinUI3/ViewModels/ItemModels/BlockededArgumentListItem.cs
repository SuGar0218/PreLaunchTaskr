using CommunityToolkit.Mvvm.ComponentModel;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class BlockedArgumentListItem : ObservableObject, IBlockedArgumentListItem
{
    public BlockedArgumentListItem(BlockedArgument argument)
    {
        this.argument = argument;
        changed = false;
    }

    public string Argument
    {
        get => argument.Argument;
        set
        {
            argument.Argument = value;
            changed = true;
        }
    }

    public bool Enabled
    {
        get => argument.Enabled;
        set
        {
            if (argument.Enabled == value)
                return;

            argument.Enabled = value;
            changed = true;
            OnPropertyChanged(nameof(Enabled));
        }
    }

    public bool IsRegex
    {
        get => argument.IsRegex;
        set
        {
            if (argument.IsRegex == value)
                return;

            argument.IsRegex = value;
            changed = true;
            OnPropertyChanged(nameof(IsRegex));
        }
    }

    /// <summary>
    /// 保存对此项的更改，但如果参数为空白，则不会保存，返回 false
    /// </summary>
    /// <returns>此项是否已保存到数据库</returns>
    public bool SaveChanges()
    {
        if (string.IsNullOrWhiteSpace(Argument))
            return false;

        if (argument.Id == -1)
            return App.Current.Configurator.BlockArgument(argument) is not null;

        if (!changed)
            return true;

        changed = false;
        return App.Current.Configurator.UpdateBlockedArgument(argument);
    }

    public bool Remove()
    {
        if (argument.Id == -1)
            return false;

        return App.Current.Configurator.RemoveBlockedArgument(argument.Id);
    }

    private readonly BlockedArgument argument;

    private bool changed;
}
