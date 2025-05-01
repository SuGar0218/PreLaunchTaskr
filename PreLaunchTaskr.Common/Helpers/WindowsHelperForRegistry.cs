using Microsoft.Win32;

namespace PreLaunchTaskr.Common.Helpers;

public partial class WindowsHelper
{
    /// <summary>
    /// 操作注册表中的 HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options
    /// <br/>
    /// 需要管理员权限来操作注册表
    /// </summary>
    public class ImageFileExecutionOptions
    {
        /// <summary>
        /// 查询 Debugger，如果此程序没有设置过 Debugger，则返回 null.
        /// </summary>
        public static string? GetDebugger(string programFileName)
        {
            using (ImageFileExecutionOptionsKey)
            {
                using RegistryKey? registryKey = ImageFileExecutionOptionsKey.OpenSubKey(programFileName);
                if (registryKey is null)
                    return null;

                object? debugger = registryKey.GetValue(DEBUGGER);
                if (debugger is null)
                    return null;

                return (string?) debugger;
            }
        }

        /// <summary>
        /// 设置 Debugger，使用前无需检查是否已存在 Debugger。
        /// </summary>
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

        /// <summary>
        /// 取消设置 Debugger，使用前无需检查是否已存在 Debugger。
        /// </summary>
        public static void UnsetDebugger(string programFileName)
        {
            using (ImageFileExecutionOptionsKey)
            {
                using RegistryKey? registryKey = ImageFileExecutionOptionsKey.OpenSubKey(programFileName, writable: true);
                if (registryKey is null || registryKey.GetValue(DEBUGGER) is null)
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
}
