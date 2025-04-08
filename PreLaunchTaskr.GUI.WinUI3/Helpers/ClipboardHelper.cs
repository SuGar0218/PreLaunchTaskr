using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

public class ClipboardHelper
{
    public static void Copy(string text)
    {
        DataPackage dataPackage = new();
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
    }

    public static async Task<string?> PasteAsync()
    {
        DataPackageView content = Clipboard.GetContent();
        if (content.Contains(StandardDataFormats.Text))
        {
            return await content.GetTextAsync();
        }
        return null;
    }
}
