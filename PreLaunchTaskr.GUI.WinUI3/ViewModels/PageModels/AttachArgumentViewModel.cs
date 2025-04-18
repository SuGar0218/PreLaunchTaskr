using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class AttachArgumentViewModel :
    ObservableObject,
    IProgramConfigCategoryViewModel,
    IAttachArgumentViewModel<AttachedArgumentListItem>
{
    public AttachArgumentViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
    }

    [ObservableProperty]
    public partial ObservableCollection<AttachedArgumentListItem>? Arguments { get; private set; }

    public void Init()
    {
        Arguments = [];
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<AttachedArgument> attachedArguments = App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id);
        foreach (AttachedArgument argument in attachedArguments)
        {
            Arguments.Add(new AttachedArgumentListItem(argument));
        }
    }

    public async Task InitAsync()
    {
        Arguments = [];
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        IList<AttachedArgument> attachedArguments = await Task.Run(() => App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id));
        foreach (AttachedArgument attachedArgument in attachedArguments)
        {
            Arguments.Add(new AttachedArgumentListItem(attachedArgument));
        }
    }

    public void AddArgument()
    {
        Arguments!.Add(new AttachedArgumentListItem(new AttachedArgument(programListItem.ProgramInfo, string.Empty, false)));
    }

    public void RemoveArgument(AttachedArgumentListItem item)
    {
        Arguments!.Remove(item);
        removed.Add(item);
    }

    public bool SaveChanges()
    {
        if (Arguments is null)
            return true;

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

    public bool IsListEmpty => Arguments is null || Arguments.Count == 0;

    private ProgramListItem programListItem;

    private readonly List<AttachedArgumentListItem> removed = [];
}
