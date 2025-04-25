using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

public struct Win32FilePickerFilter
{
    public Win32FilePickerFilter(string type, string extension)
    {
        Type = type;
        Extension = extension;
    }

    public string Type { get; }

    public string Extension { get; }
}
