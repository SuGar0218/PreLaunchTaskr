using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32.Foundation;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class ProgramListItem : ObservableObject, IProgramListItem<BitmapImage>
{
    public int Id => ProgramInfo.Id;

    public string Name { get; init; }

    public string Path => ProgramInfo.Path;

    public BitmapImage? Icon { get; init; }

    public bool Enabled
    {
        get => ProgramInfo.Enabled;
        set
        {
            if (ProgramInfo.Enabled != value)
            {
                ProgramInfo.Enabled = value;
                changed = true;
                SaveChanges();
            }
            OnPropertyChanged(nameof(Enabled));
        }
    }

    public ProgramListItem(ProgramInfo programInfo)
    {
        ProgramInfo = programInfo;
        Name = System.IO.Path.GetFileName(programInfo.Path)!;
        Icon = IconBitmapImageReader.ReadFromExe(programInfo.Path) ?? defaultProgramIcon;
        changed = false;
    }

    private bool SaveChanges()
    {
        // 不调用配置器的方法是因为本程序通常不会在管理员模式下运行，
        // 也不能在管理员模式下运行，因为会导致 FilePicker 无法打开。
        if (!changed)
            return true;

        changed = false;
        string commandArgs = Enabled ? $"-s enable-program --id {Id}" : $"-s disable-program --id {Id}";
        try
        {
            return ProcessStarter.StartSilentAsAdminAndWait(Properties.ConfiguratorNet8Location, commandArgs) is not null;
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

        App.Current.Configurator.RemoveAttachedArgument(ProgramInfo.Id);
        return App.Current.Configurator.RemoveProgram(ProgramInfo.Id);
    }

    public ProgramInfo ProgramInfo { get; private set; }

    private bool changed;

    private static readonly BitmapImage defaultProgramIcon = new(new Uri(System.IO.Path.Combine(App.BaseUri, @"Assets\DefaultProgramIcon.png")));
}
