using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Utils;

/// <summary>
/// 操作注册表中的 HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options
/// <br/>
/// 需要管理员权限来操作注册表
/// </summary>
public class ImageFileExecutionOptions
{
    public static void SetDebugger(string programFileName, string debuggerCommandString)
    {
        using (ImageFileExecutionOptionsKey)
        {
            using RegistryKey registryKey =
                ImageFileExecutionOptionsKey.OpenSubKey(programFileName, writable: true) ??
                ImageFileExecutionOptionsKey.CreateSubKey(programFileName, writable: true);
            registryKey.SetValue(DEBUGGER, debuggerCommandString);
        }
    }

    public static void UnsetDebugger(string programFileName)
    {
        using (ImageFileExecutionOptionsKey)
        {
            using RegistryKey? registryKey = ImageFileExecutionOptionsKey.OpenSubKey(programFileName, writable: true);
            if (registryKey is null)
                return;
             registryKey.DeleteValue(DEBUGGER);
        }
    }

    /// <summary>
    /// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options
    /// </summary>
    private static RegistryKey ImageFileExecutionOptionsKey => Registry.LocalMachine
        .OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", writable: true)!;

    private const string DEBUGGER = "Debugger";
}
