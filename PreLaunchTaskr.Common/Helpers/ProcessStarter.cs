using System.ComponentModel;
using System.Diagnostics;

namespace PreLaunchTaskr.Common;

public class ProcessStarter
{
    /// <summary>
    /// 静默启动进程
    /// </summary>
    /// <exception cref="InvalidOperationException">未指定文件名</exception>
    /// <exception cref="Win32Exception">打开关联的文件时出错、找不到指定文件</exception>
    /// <exception cref="PlatformNotSupportedException">只有 Windows 支持</exception>
    /// <returns></returns>
    public static Process? StartSilentAsAdmin(string path, string arguments = "")
    {
        Process process = new();
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Verb = "runas";  // 以管理员身份运行
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UserName = null;
        if (!process.Start())
            return null;
        return process;
    }

    /// <summary>
    /// 静默启动进程
    /// </summary>
    /// <exception cref="InvalidOperationException">未指定文件名</exception>
    /// <exception cref="Win32Exception">打开关联的文件时出错、找不到指定文件</exception>
    /// <exception cref="PlatformNotSupportedException">只有 Windows 支持</exception>
    /// <returns></returns>
    public static Process? StartSilent(string path, string arguments = "")
    {
        Process process = new();
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UserName = null;
        if (!process.Start())
            return null;
        return process;
    }

    public static Process? Start(string path, string arguments = "")
    {
        Process process = new();
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UserName = null;
        if (!process.Start())
            return null;
        return process;
    }

    public static Process? StartAsAdmin(string path, string arguments = "")
    {
        Process process = new();
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Verb = "runas";  // 以管理员身份运行
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UserName = null;
        if (!process.Start())
            return null;
        return process;
    }

    /// <summary>
    /// 静默启动进程
    /// </summary>
    /// <exception cref="InvalidOperationException">未指定文件名</exception>
    /// <exception cref="Win32Exception">打开关联的文件时出错、找不到指定文件</exception>
    /// <exception cref="PlatformNotSupportedException">只有 Windows 支持</exception>
    /// <returns></returns>
    public static Process? StartSilentAsAdminAndWait(string path, string arguments = "")
    {
        Process process = new();
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Verb = "runas";  // 以管理员身份运行
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UserName = null;
        if (!process.Start())
            return null;
        process.WaitForExit();
        return process;
    }

    /// <summary>
    /// 静默启动进程
    /// </summary>
    /// <exception cref="InvalidOperationException">未指定文件名</exception>
    /// <exception cref="Win32Exception">打开关联的文件时出错、找不到指定文件</exception>
    /// <exception cref="PlatformNotSupportedException">只有 Windows 支持</exception>
    /// <returns></returns>
    public static Process? StartSilentAndWait(string path, string arguments = "")
    {
        Process process = new();
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UserName = null;
        if (!process.Start())
            return null;
        process.WaitForExit();
        return process;
    }
}
