using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Utils;

using System;
using System.IO;

using static PreLaunchTaskr.Common.Helpers.WindowsHelper;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class InstalledTraditionalProgramListItem : IInstalledProgramListItem<BitmapImage>
{
    public string Name => programUninstallInfo.DisplayName ?? string.Empty;

    public BitmapImage Icon { get; }

    public string Version => programUninstallInfo.DisplayVersion ?? string.Empty;

    public string Publisher => programUninstallInfo.Publisher ?? string.Empty;

    public string PossiblePath { get; }

    public InstalledTraditionalProgramListItem(ProgramUninstallInfo programUninstallInfo)
    {
        this.programUninstallInfo = programUninstallInfo;

        if (!string.IsNullOrWhiteSpace(programUninstallInfo.InstallLocation))
        {
            PossiblePath = StringHelper.TrimQuotes(programUninstallInfo.InstallLocation);
        }
        else if (!string.IsNullOrWhiteSpace(programUninstallInfo.UninstallString))
        {
            // UninstallString 可能有以下形式：
            // Disk:\path with spaces\to\un installer.exe
            // Disk:\path_without_spaces\to\un_installer.exe args
            // "Disk:\path with spaces\to\un installer.exe"
            // "Disk:\path with spaces\to\un installer.exe" args
            if (programUninstallInfo.UninstallString[0] == '\"' || programUninstallInfo.UninstallString[0] == '\'')
            {
                PossiblePath = Path.GetDirectoryName(StringHelper.GetFirstToken(programUninstallInfo.UninstallString)) ?? string.Empty;
            }
            else
            {
                PossiblePath = Path.GetDirectoryName(programUninstallInfo.UninstallString) ?? string.Empty;
                if (!Directory.Exists(PossiblePath))
                    PossiblePath = string.Empty;
            }
        }
        else if (!string.IsNullOrWhiteSpace(programUninstallInfo.DisplayIcon))
        {
            PossiblePath = Path.GetDirectoryName(StringHelper.TrimQuotes(programUninstallInfo.DisplayIcon.Split(',')[0])) ?? string.Empty;
        }
        else
        {
            PossiblePath = string.Empty;
        }

        string? displayIcon = programUninstallInfo.DisplayIcon;
        if (displayIcon is null)
        {
            Icon = defaultProgramIcon;
            return;
        }
        string[] split = displayIcon.Split(',');
        try
        {
            if (split[0].EndsWith('l'))
            {
                Icon = IconBitmapImageReader.ReadFromDll(split[0], int.Parse(split[1])) ?? defaultProgramIcon;
            }
            else
            {
                Icon = IconBitmapImageReader.ReadAssociated(split[0]) ?? defaultProgramIcon;
            }
        }
        catch (Exception)
        {
            Icon = defaultProgramIcon;
        }
    }

    private readonly ProgramUninstallInfo programUninstallInfo;

    private static readonly BitmapImage defaultProgramIcon = new(new Uri(System.IO.Path.Combine(App.BaseDirectory, @"Assets\DefaultProgramIcon.png")));
}
