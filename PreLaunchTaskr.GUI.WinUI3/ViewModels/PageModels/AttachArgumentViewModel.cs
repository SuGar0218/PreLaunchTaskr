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

public partial class AttachArgumentViewModel : ObservableObject, IProgramConfigCategoryViewModel, IAttachArgumentViewModel<DispatcherQueue, AttachedArgumentListItem>
{
    public AttachArgumentViewModel(ProgramListItem programListItem)
    {
        this.programListItem = programListItem;
        Arguments = null!;
    }

    [ObservableProperty]
    public partial ObservableCollection<AttachedArgumentListItem> Arguments { get; private set; }

    public void Init()
    {
        //Arguments.Clear();
        Arguments = [];
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        foreach (AttachedArgument argument in App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id))
        {
            Arguments.Add(new AttachedArgumentListItem(argument));
        }
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public async Task InitAsync(DispatcherQueue dispatcherQueue)
    {
        Arguments = [];
        Arguments.CollectionChanged += (o, e) => OnPropertyChanged(nameof(IsListEmpty));
        removed.Clear();
        await Task.Run(() =>
        {
            //dispatcherQueue.TryEnqueue(Arguments.Clear);
            //foreach (AttachedArgument argument in App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id))
            //{
            //    dispatcherQueue.TryEnqueue(() => Arguments.Add(new AttachedArgumentListItem(argument)));
            //}
            IList<AttachedArgument> attachedArguments = App.Current.Configurator.ListAttachedArgumentsForProgram(programListItem.Id);
            dispatcherQueue.TryEnqueue(() =>
            {
                foreach (AttachedArgument attachedArgument in attachedArguments)
                {
                    Arguments.Add(new AttachedArgumentListItem(attachedArgument));
                }
            });
        });
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public void AddArgument()
    {
        Arguments.Add(new AttachedArgumentListItem(new AttachedArgument(programListItem.ProgramInfo, string.Empty, false)));
        //OnPropertyChanged(nameof(IsListEmpty));
    }

    public void RemoveArgument(AttachedArgumentListItem item)
    {
        Arguments.Remove(item);
        removed.Add(item);
        //OnPropertyChanged(nameof(IsListEmpty));
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
