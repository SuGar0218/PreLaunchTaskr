using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;

namespace PreLaunchTaskr.GUI.Common.Helpers;

public class WindowsHelper
{
    /// <summary>
    /// 在资源管理器中打开并选择文件
    /// <br/>
    /// 此功能通过调用 SHOpenFolderAndSelectItems 实现
    /// </summary>
    public static bool OpenPathInExplorer(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            return false;

        unsafe
        {
            Windows.Win32.UI.Shell.Common.ITEMIDLIST* pItemIdList = PInvoke.ILCreateFromPath(path);
            if (pItemIdList is null)
                return false;

            PInvoke.CoInitialize(null);
            HRESULT hResult = PInvoke.SHOpenFolderAndSelectItems(pItemIdList, 0, null, 0);
            PInvoke.CoUninitialize();
            PInvoke.ILFree(pItemIdList);
            return hResult.Succeeded;
        }
    }

    /// <summary>
    /// 列出已安装的程序所在路径
    /// <br/>
    /// 此功能通过读取注册表实现
    /// <br/>
    /// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths
    /// </summary>
    /// <returns></returns>
    //public static string[] ListInstalledAppPaths()
    //{
    //    using RegistryKey appPathsKey = Registry.LocalMachine
    //        .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths")!;
    //    string[] paths = new string[appPathsKey.SubKeyCount];
    //    string[] subKeyNames = appPathsKey.GetSubKeyNames();
    //    for (int i = 0; i < subKeyNames.Length; i++)
    //    {
    //        using RegistryKey appKey = appPathsKey.OpenSubKey(subKeyNames[i])!;

    //    }
    //}
}
