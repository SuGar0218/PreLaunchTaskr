using Microsoft.UI.Xaml.Media.Imaging;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PreLaunchTaskr.GUI.WinUI3.Utils;

public class IconBitmapImageReader
{
    public static BitmapImage? ReadAssociated(string path)
    {
        if (!File.Exists(path))
            return null;

        Icon? icon = Icon.ExtractAssociatedIcon(path);

        if (icon is null)
            return null;

        return IconToBitmapImage(icon);
    }

    public static BitmapImage? ReadFromDll(string path, int id)
    {
        if (!File.Exists(path))
            return null;

        Icon? icon = Icon.ExtractIcon(path, id);

        if (icon is null)
            return null;

        return IconToBitmapImage(icon);
    }

    private static BitmapImage IconToBitmapImage(Icon icon)
    {
        using MemoryStream stream = new();
        icon.ToBitmap().Save(stream, ImageFormat.Png);
        stream.Position = 0;
        BitmapImage bitmapImage = new();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }
}
