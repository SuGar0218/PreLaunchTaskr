using Microsoft.Win32;

using PreLaunchTaskr.Common.Converters;

namespace PreLaunchTaskr.Common.Helpers;

public partial class WindowsHelper
{
    public class ProgramUninstallInfo
    {
        public string? DisplayName { get; set; }

        public string? DisplayIcon { get; set; }

        public string? DisplayVersion { get; set; }

        public string? Publisher { get; set; }

        public string? InstallLocation { get; set; }

        public string? UninstallString { get; set; }
    }

    public static List<ProgramUninstallInfo> ListProgramUninstallInfo()
    {
        List<ProgramUninstallInfo> infos = new();

        using RegistryKey? uninstallKey64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
        if (uninstallKey64 is not null)
        {
            string[] subKeyNames = uninstallKey64.GetSubKeyNames();
            for (int i = 0; i < subKeyNames.Length; i++)
            {
                ProgramUninstallInfo? info = ReadProgramUninstallInfo(uninstallKey64.OpenSubKey(subKeyNames[i])!);
                if (info is not null)
                    infos.Add(info);
            }
        }

        using RegistryKey? uninstallKey32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
        if (uninstallKey32 is not null)
        {
            string[] subKeyNames = uninstallKey32.GetSubKeyNames();
            for (int i = 0; i < subKeyNames.Length; i++)
            {
                ProgramUninstallInfo? info = ReadProgramUninstallInfo(uninstallKey32.OpenSubKey(subKeyNames[i])!);
                if (info is not null)
                    infos.Add(info);
            }
        }

        return infos;
    }

    /// <summary>
    /// 此函数会 using RegistryKey，会正确释放资源。
    /// </summary>
    private static ProgramUninstallInfo? ReadProgramUninstallInfo(RegistryKey key)
    {
        using (key)
        {
            if (key.GetValue("DisplayName") is string displayName)
                displayName = RegSzToStringConverter.ConvertAndTrimQuotation(displayName);
            else
                //displayName = null!;
                return null;

            if (key.GetValue("DisplayIcon") is string displayIcon)
                displayIcon = RegSzToStringConverter.ConvertAndTrimQuotation(displayIcon);
            else
                displayIcon = null!;

            if (key.GetValue("DisplayVersion") is string displayVersion)
                displayVersion = RegSzToStringConverter.ConvertAndTrimQuotation(displayVersion);
            else
                displayVersion = null!;

            if (key.GetValue("Publisher") is string publisher)
                publisher = RegSzToStringConverter.ConvertAndTrimQuotation(publisher);
            else
                publisher = null!;

            if (key.GetValue("InstallLocation") is string installLocation)
                installLocation = RegSzToStringConverter.ConvertAndTrimQuotation(installLocation);
            else
                installLocation = null!;

            if (key.GetValue("UninstallString") is string uninstallString)
                uninstallString = RegSzToStringConverter.ConvertAndTrimQuotation(uninstallString);
            else
                uninstallString = null!;

            return new ProgramUninstallInfo
            {
                DisplayName = displayName,
                DisplayIcon = displayIcon,
                DisplayVersion = displayVersion,
                Publisher = publisher,
                InstallLocation = installLocation,
                UninstallString = uninstallString
            };
        }
    }
}
