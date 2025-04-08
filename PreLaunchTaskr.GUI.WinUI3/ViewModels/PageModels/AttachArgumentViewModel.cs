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

public class AttachArgumentViewModel : ObservableObject, IProgramConfigCategoryViewModel
{
    public AttachArgumentViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
    }

    public ObservableCollection<AttachedArgumentListItem> Arguments { get; } = [];

    public void Init()
    {
        Arguments.Clear();
        removed.Clear();
        foreach (AttachedArgument argument in App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id))
        {
            Arguments.Add(new AttachedArgumentListItem(argument));
        }
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(Arguments.Clear);
            foreach (AttachedArgument argument in App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id))
            {
                dispatcherQueue.TryEnqueue(() => Arguments.Add(new AttachedArgumentListItem(argument)));
            }
        });
    }

    public void AddArgument()
    {
        Arguments.Add(new AttachedArgumentListItem(new AttachedArgument(programListItem.ProgramInfo, string.Empty, false)));
    }

    public void RemoveArgument(AttachedArgumentListItem item)
    {
        Arguments.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
        foreach (AttachedArgumentListItem item in Arguments)
        {
            item.SaveChanges();
        }
        foreach (AttachedArgumentListItem item in removed)
        {
            item.Remove();
        }
        removed.Clear();
        return true;
    }

    public bool IsListEmpty => Arguments.Count == 0;

    private ProgramListItem programListItem;

    private readonly List<AttachedArgumentListItem> removed = [];
}
