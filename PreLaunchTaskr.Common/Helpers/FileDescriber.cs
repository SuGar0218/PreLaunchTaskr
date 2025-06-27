using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Common.Helpers;

/// <summary>
/// 对指定路径的文件获取描述文本
/// </summary>
public class FileDescriber
{
    public static string Describe(string path)
    {
        FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);

        if (!string.IsNullOrWhiteSpace(info.FileDescription))
            return info.FileDescription;

        if (!string.IsNullOrWhiteSpace(info.ProductName))
            return info.ProductName;

        return System.IO.Path.GetFileName(path);
    }
}
