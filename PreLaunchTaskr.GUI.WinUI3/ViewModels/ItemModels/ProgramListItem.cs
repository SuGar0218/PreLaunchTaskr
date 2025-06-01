using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.Common;
using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Utils;

using System;
using System.Collections.Generic;
using System.Text;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public partial class ProgramListItem : ObservableObject, IProgramListItem<BitmapImage>
{
    public ProgramListItem(ProgramInfo programInfo)
    {
        ProgramInfo = programInfo;
        Name = GetProgramNameFromPath(programInfo.Path)!;
        Icon = IconBitmapImageReader.ReadAssociated(programInfo.Path) ?? defaultProgramIcon;
        changed = false;
    }

    private static string GetProgramNameFromPath(string path)
    {
        if (System.IO.File.Exists(path) && System.IO.Path.GetExtension(path).ToLowerInvariant() == ".exe")
        {
            var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(path);
            
            // 优先使用FileDescription，如果为空则使用ProductName，最后使用文件名
            if (!string.IsNullOrWhiteSpace(info.FileDescription))
                return info.FileDescription;

            if (!string.IsNullOrWhiteSpace(info.ProductName))
                return info.ProductName;
        }
        return System.IO.Path.GetFileName(path);
    }
    
    public int Id => ProgramInfo.Id;

    public string Name { get; init; }

    public string Path => ProgramInfo.Path;

    public BitmapImage? Icon { get; init; }

    /// <summary>
    /// 是否启用对此程序的设置
    /// <br/>
    /// 更改此属性不会立即更改注册表映像劫持
    /// </summary>
    public bool Enabled
    {
        get => ProgramInfo.Enabled;
        set
        {
            if (value == Enabled)
                return;

            ProgramInfo.Enabled = value;
            changed = true;
            OnPropertyChanged(nameof(Enabled));
        }
    }

    public bool SaveChanges()
    {
        // 不调用配置器的方法是因为本程序通常不会在管理员模式下运行，
        // 也不能在管理员模式下运行，因为会导致 FilePicker 无法打开。
        if (!changed)
            return true;

        changed = false;
        string commandArgs = Enabled ? $"-s enable-program --id {Id}" : $"-s disable-program --id {Id}";
        try
        {
            return ProcessStarter.StartSilentAsAdminAndWait(
                System.IO.Path.Combine(App.BaseDirectory, GlobalProperties.ConfiguratorNet8Location),
                commandArgs) is not null;
        }
        catch
        {
            return false;
        }
    }

    public bool Remove()
    {
        if (ProgramInfo.Id == -1)
            return false;

        Enabled = false;
        return App.Current.Configurator.RemoveProgram(ProgramInfo.Id);
    }

    /// <summary>
    /// 批量保存修改。如果对每一项修改，会造成连续弹出用户账户控制。
    /// </summary>
    /// <param name="items">要保存的项</param>
    /// <param name="backup">如果失败则恢复到这些值，也可以调用前后自行处理。</param>
    /// <returns>是否保存成功</returns>
    public static bool SaveChanges(IEnumerable<ProgramListItem> items, bool[]? backup = null)
    {
        StringBuilder enableArgsBuilder = new();
        StringBuilder disableArgsBuilder = new();
        foreach (ProgramListItem item in items)
        {
            if (item.changed)
            {
                item.changed = false;
                if (item.Enabled)
                {
                    enableArgsBuilder.Append(" --enable ").Append(item.Id);
                }
                else
                {
                    disableArgsBuilder.Append(" --disable ").Append(item.Id);
                }
            }
        }

        if (enableArgsBuilder.Length == 0 && disableArgsBuilder.Length == 0)
            return true;

        bool success;
        try
        {
            success = ProcessStarter.StartSilentAsAdminAndWait(
                System.IO.Path.GetFullPath(GlobalProperties.ConfiguratorNet8Location),
                " -s " +
                enableArgsBuilder.ToString() +
                disableArgsBuilder.ToString()) is not null;
        }
        catch
        {
            success = false;
        }
        if (!success && backup is not null)
        {
            int i = 0;
            foreach (ProgramListItem item in items)
            {
                item.Enabled = backup[i];
                i++;
            }
        }
        return success;
    }

    /// <summary>
    /// 批量移除。如果对每一项移除，会造成连续弹出用户账户控制。
    /// </summary>
    /// <param name="items">要保存的项</param>
    /// <returns>是否移除成功</returns>
    //public static bool Remove(IEnumerable<ProgramListItem> items)
    //{

    //}

    public ProgramInfo ProgramInfo { get; private set; }

    private bool changed;

    private static readonly BitmapImage defaultProgramIcon = new(new Uri(System.IO.Path.Combine(App.BaseDirectory, @"Assets\DefaultProgramIcon.png")));

    public override string ToString()
    {
        return $"{Name}, {Enabled}";
    }
}
