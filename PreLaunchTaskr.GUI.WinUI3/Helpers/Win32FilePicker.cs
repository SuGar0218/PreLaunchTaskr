using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Controls.Dialogs;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

public class Win32FilePicker
{
    public static async Task<string?> PickFileAsync(
        IEnumerable<Win32FilePickerFilter> filters,
        string initialDirectory,
        bool dereferenceLink,
        nint hWnd)
    {
        return await Task.Run(() => PickFile(filters, initialDirectory, dereferenceLink, hWnd));
    }

    public static string? PickFile(
        IEnumerable<Win32FilePickerFilter> filters,
        string initialDirectory,
        bool dereferenceLink,
        nint hWnd)
    {
        OPENFILENAMEW openFileNameW = new()
        {
            lStructSize = (uint) Marshal.SizeOf<OPENFILENAMEW>(),
            hwndOwner = new HWND(hWnd),
            hInstance = HINSTANCE.Null,
            nMaxCustFilter = 40,
            nFilterIndex = 0,
            nMaxFile = 256,
            nMaxFileTitle = 0,
            Flags = OPEN_FILENAME_FLAGS.OFN_EXPLORER
        };
        if (!dereferenceLink)
        {
            openFileNameW.Flags |= OPEN_FILENAME_FLAGS.OFN_NODEREFERENCELINKS;
        }
        StringBuilder filterString = new();
        foreach (Win32FilePickerFilter filter in filters)
        {
            filterString.Append(filter.Type).Append('\0');
            filterString.Append(filter.Extension).Append('\0');
        }
        filterString.Append('\0');
        unsafe
        {
            fixed (char* lpstrFilter = filterString.ToString().ToArray())
            fixed (char* lpstrInitialDir = initialDirectory.ToArray())
            fixed (char* lpstrTitle = "选择一个程序或程序的快捷方式".ToArray())
            fixed (char* lpstrFileTitle = "\0".ToArray())
            fixed (char* lpstrFile = new char[openFileNameW.nMaxFile])  // 不直接用"\0"字符串是为了预留空间，防止稍后意外改变了长度
            {
                lpstrFile[0] = '\0';  // 微软文档里要求以'\0'字符开头
                PWSTR nullPWSTR = new(null);
                openFileNameW.lpstrFilter = lpstrFilter;
                openFileNameW.lpstrCustomFilter = nullPWSTR;
                openFileNameW.lpstrFile = lpstrFile;
                openFileNameW.lpstrFileTitle = nullPWSTR;
                openFileNameW.lpstrTitle = lpstrTitle;
                openFileNameW.lpstrInitialDir = lpstrInitialDir;
            }
        }
        //if (!PInvoke.GetOpenFileName(ref openFileNameW))
        //{
        //    //throw new Win32Exception(PInvoke.CommDlgExtendedError().ToString());
        //}
        //return openFileNameW.lpstrFile.ToString();
        return PInvoke.GetOpenFileName(ref openFileNameW) ? openFileNameW.lpstrFile.ToString() : null;
    }
}
