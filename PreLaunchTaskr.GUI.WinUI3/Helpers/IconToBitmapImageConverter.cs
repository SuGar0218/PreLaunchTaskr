using Microsoft.UI.Xaml.Media.Imaging;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.Utils;

public class IconToBitmapImageConverter
{
    public static BitmapImage Convert(Icon icon)
    {
        MemoryStream stream = new();
        icon.ToBitmap().Save(stream, ImageFormat.Png);
        stream.Position = 0;
        BitmapImage bitmapImage = new();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }
}
