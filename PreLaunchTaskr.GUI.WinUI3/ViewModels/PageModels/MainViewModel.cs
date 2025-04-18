using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.Common;
using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public class MainViewModel : IMainViewModel<ProgramListItem, BitmapImage>
{
    public ObservableCollection<ProgramListItem> Programs { get; private set; } = [];

    public void Init()
    {
        Programs.Clear();
        idCache.Clear();
        nameCache.Clear();
        IList<ProgramInfo> programInfos = App.Current.Configurator.ListPrograms();
        foreach (ProgramInfo programInfo in App.Current.Configurator.ListPrograms())
        {
            ProgramListItem newProgramListItem = new(programInfo);
            Programs.Add(newProgramListItem);
            idCache.Add(programInfo.Id, newProgramListItem);
            nameCache.Add(newProgramListItem.Name, newProgramListItem);
        }
    }

    public async Task InitAsync()
    {
        Programs.Clear();
        idCache.Clear();
        nameCache.Clear();
        IList<ProgramInfo> programInfos = await Task.Run(() => App.Current.Configurator.ListPrograms());
        foreach (ProgramInfo programInfo in App.Current.Configurator.ListPrograms())
        {
            ProgramListItem newProgramListItem = new(programInfo);
            Programs.Add(newProgramListItem);
            idCache.Add(programInfo.Id, newProgramListItem);
            nameCache.Add(newProgramListItem.Name, newProgramListItem);
        }
    }

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

    public bool RemoveProgram(ProgramListItem item)
    {
        Programs.Remove(item);
        idCache.Remove(item.Id);
        nameCache.Remove(item.Name);
        return item.Remove();
    }

    public bool EnableProgram(int id)
    {
        if (!idCache.ContainsKey(id))
            return false;

        return ProcessStarter.StartSilentAsAdminAndWait(GlobalProperties.ConfiguratorNet8Location, $" enable-program --id {id}") is not null;
    }

    public bool DisableProgram(int id)
    {
        if (!idCache.ContainsKey(id))
            return false;

        return ProcessStarter.StartSilentAsAdminAndWait(GlobalProperties.ConfiguratorNet8Location, $" disable-program --id {id}") is not null;
    }

    private readonly Dictionary<int, ProgramListItem> idCache = new();
    private readonly Dictionary<string, ProgramListItem> nameCache = new();
}
