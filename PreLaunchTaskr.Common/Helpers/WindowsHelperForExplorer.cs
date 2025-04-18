using Windows.Win32;
using Windows.Win32.Foundation;

namespace PreLaunchTaskr.Common.Helpers;

public partial class WindowsHelper
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
}
