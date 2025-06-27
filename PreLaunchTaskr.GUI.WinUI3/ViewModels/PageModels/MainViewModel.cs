using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.Common;
using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class MainViewModel : ObservableObject, IMainViewModel<ProgramListItem, BitmapImage>
{
    [ObservableProperty]
    public partial ObservableCollection<ProgramListItem> Programs { get; private set; }

    [ObservableProperty]
    public partial ProgramListItem SelectedItem { get; set; }

    public void Init()
    {
        Programs = [];
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
        Programs = [];
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

    //public bool AddProgram(string path)
    //{
    //    string name = FileDescriber.Describe(path);
    //    return AddProgram(name, path);
    //}

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

    public async Task<bool> EnableAllPrograms(bool enable = true)
    {
        int i;
        bool[] backup = new bool[Programs.Count];  // 复制一份，如果未能修改成功，则回退
        i = 0;
        foreach (ProgramListItem item in Programs)
        {
            backup[i] = item.Enabled;
            item.Enabled = enable;
            i++;
        }
        bool success = await Task.Run(() => ProgramListItem.SaveChanges(Programs, backup));
        //if (!success)
        //{
        //    i = 0;
        //    foreach (ProgramListItem item in Programs)
        //    {
        //        item.Enabled = backup[i];
        //        i++;
        //    }
        //}
        return success;
    }

    public async Task<bool> RemoveAllPrograms()
    {
        if (! await EnableAllPrograms(false))
            return false;

        foreach (ProgramListItem item in Programs)
        {
            item.Remove();
        }

        // 不能用 foreach 移除
        Programs.Clear();
        idCache.Clear();
        nameCache.Clear();
        return true;
    }

    private readonly Dictionary<int, ProgramListItem> idCache = new();
    private readonly Dictionary<string, ProgramListItem> nameCache = new();
}
