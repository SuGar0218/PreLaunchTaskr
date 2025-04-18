using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace PreLaunchTaskr.GUI.WPF.Utils;

public class IconBitmapImageReader
{
    public static BitmapImage? ReadFromExe(string path)
    {
        if (!File.Exists(path))
            return null;

        Icon? icon = Icon.ExtractAssociatedIcon(path);

        if (icon is null)
            return null;

        return IconToBitmapImage(icon);
    }

    //public static BitmapImage? ReadFromDll(string path, int id)
    //{
    //    if (!File.Exists(path))
    //        return null;

    //    Icon? icon = Icon.ExtractIcon(path, id);

    //    if (icon is null)
    //        return null;

    //    return IconToBitmapImage(icon);
    //}

    private static BitmapImage IconToBitmapImage(Icon icon)
    {
        using MemoryStream stream = new();
        icon.ToBitmap().Save(stream, ImageFormat.Png);
        stream.Position = 0;
        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        return bitmapImage;
    }
}
