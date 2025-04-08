using Microsoft.UI.Xaml.Media.Imaging;

using System;
using System.Drawing;
using System.IO;

namespace PreLaunchTaskr.GUI.WinUI3.Utils;

public class IconBitmapImageReader
{
    public static BitmapImage? ReadFromExe(string path)
    {
        if (!File.Exists(path))
            return null;

        Icon? icon = Icon.ExtractAssociatedIcon(path);

        if (icon is null)
            return null;

        return IconToBitmapImageConverter.Convert(icon);
    }

    public static BitmapImage? ReadFromDll(string path, int id)
    {
        if (!File.Exists(path))
            return null;

        Icon? icon = Icon.ExtractIcon(path, id);

        if (icon is null)
            return null;

        return IconToBitmapImageConverter.Convert(icon);
    }
}
