using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class BlockArgumentViewModel :
    ObservableObject,
    IProgramConfigCategoryViewModel,
    IBlockArgumentViewModel<BlockedArgumentListItem>
{
    public BlockArgumentViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
    }

    [ObservableProperty]
    public partial ObservableCollection<BlockedArgumentListItem>? Arguments { get; private set; }

    public void Init()
    {
        Arguments = [];
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<BlockedArgument> blockedArguments = App.Current.Configurator.ListBlockedArgumentsForProgram(programListItem.Id);
        foreach (BlockedArgument blockedArgument in blockedArguments)
        {
            Arguments.Add(new BlockedArgumentListItem(blockedArgument));
        }
    }

    public async Task InitAsync()
    {
        Arguments = [];
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<BlockedArgument> blockedArguments = await Task.Run(() => App.Current.Configurator.ListBlockedArgumentsForProgram(programListItem.Id));
        foreach (BlockedArgument blockedArgument in blockedArguments)
        {
            Arguments.Add(new BlockedArgumentListItem(blockedArgument));
        }
    }

    public void AddArgument()
    {
        Arguments!.Add(new BlockedArgumentListItem(new BlockedArgument(programListItem.ProgramInfo, string.Empty, false, false)));
    }

    public void RemoveArgument(BlockedArgumentListItem item)
    {
        Arguments!.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
        if (Arguments is null)
            return true;

        foreach (BlockedArgumentListItem item in Arguments)
        {
            item.SaveChanges();
        }
        foreach (BlockedArgumentListItem item in removed)
        {
            item.Remove();
        }
        removed.Clear();
        return true;
    }

    public bool IsListEmpty => Arguments is null || Arguments.Count == 0;

    private ProgramListItem programListItem;

    private readonly List<BlockedArgumentListItem> removed = [];
}
