using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public class BlockArgumentViewModel : ObservableObject, IProgramConfigCategoryViewModel
{
    public BlockArgumentViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
    }

    public ObservableCollection<BlockedArgumentListItem> Arguments { get; } = [];

    public void Init()
    {
        Arguments.Clear();
        removed.Clear();
        foreach (BlockedArgument argument in App.Current.Configurator.ListBlockedArgumentsForProgram(programListItem.Id))
        {
            Arguments.Add(new BlockedArgumentListItem(argument));
        }
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(Arguments.Clear);
            foreach (BlockedArgument argument in App.Current.Configurator.ListBlockedArgumentsForProgram(programListItem.Id))
            {
                dispatcherQueue.TryEnqueue(() => Arguments.Add(new BlockedArgumentListItem(argument)));
            }
        });
    }

    public void AddArgument()
    {
        Arguments.Add(new BlockedArgumentListItem(new BlockedArgument(programListItem.ProgramInfo, string.Empty, false, false)));
    }

    public void RemoveArgument(BlockedArgumentListItem item)
    {
        Arguments.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
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

    public bool IsListEmpty => Arguments.Count == 0;

    private ProgramListItem programListItem;

    private readonly List<BlockedArgumentListItem> removed = [];
}
