using PreLaunchTaskr.Common;
using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;

using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

public class MainViewModel : IMainViewModel<ProgramListItem, BitmapImage>
{
    public ObservableCollection<ProgramListItem> Programs { get; } = new();

    public bool AddProgram(string name, string path)
    {
        if (nameCache.ContainsKey(name))
            return false;

        ProgramInfo? newProgramInfo = new(path, false);
        newProgramInfo = App.Current.Configurator.AddProgram(newProgramInfo);
        if (newProgramInfo is null)
            return false;

        ProgramListItem newProgramListItem = new(newProgramInfo);
        Programs.Add(newProgramListItem);
        idCache.Add(newProgramInfo.Id, newProgramListItem);
        nameCache.Add(newProgramListItem.Name, newProgramListItem);
        return true;
    }

    public bool EnableProgram(int id)
    {
        if (!idCache.ContainsKey(id))
            return false;

        return ProcessStarter.StartSilentAsAdminAndWait(GlobalProperties.ConfiguratorNet6Location, $" enable-program --id {id}") is not null;
    }

    public bool DisableProgram(int id)
    {
        if (!idCache.ContainsKey(id))
            return false;

        return ProcessStarter.StartSilentAsAdminAndWait(GlobalProperties.ConfiguratorNet6Location, $" disable-program --id {id}") is not null;
    }

    public void Init()
    {
        Programs.Clear();
        idCache.Clear();
        nameCache.Clear();
        IList<ProgramInfo> programInfos = App.Current.Configurator.ListPrograms();
        foreach (ProgramInfo programInfo in programInfos)
        {
            ProgramListItem newProgramListItem = new(programInfo);
            Programs.Add(newProgramListItem);
            idCache.Add(newProgramListItem.Id, newProgramListItem);
            nameCache.Add(newProgramListItem.Name, newProgramListItem);
        }
    }

    public async Task InitAsync()
    {
        Programs.Clear();
        idCache.Clear();
        nameCache.Clear();
        IList<ProgramInfo> programInfos = await Task.Run(() => App.Current.Configurator.ListPrograms());
        foreach (ProgramInfo programInfo in programInfos)
        {
            ProgramListItem newProgramListItem = new(programInfo);
            Programs.Add(newProgramListItem);
            idCache.Add(newProgramListItem.Id, newProgramListItem);
            nameCache.Add(newProgramListItem.Name, newProgramListItem);
        }
    }

    public bool RemoveProgram(ProgramListItem item)
    {
        Programs.Remove(item);
        idCache.Remove(item.Id);
        nameCache.Remove(item.Name);
        return item.Remove();
    }

    private readonly Dictionary<int, ProgramListItem> idCache = new();
    private readonly Dictionary<string, ProgramListItem> nameCache = new();
}
