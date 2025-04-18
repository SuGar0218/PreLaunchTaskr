using System.Windows;

namespace PreLaunchTaskr.GUI.WPF.Helpers;

public class ClipboardHelper
{
    public static void Copy(string text)
    {
        Clipboard.SetDataObject(text);
    }

    //public static async Task<string?> PasteAsync()
    //{
    //    //DataPackageView content = Clipboard.GetContent();
    //    //if (content.Contains(StandardDataFormats.Text))
    //    //{
    //    //    return await content.GetTextAsync();
    //    //}
    //    return null;
    //}
}
