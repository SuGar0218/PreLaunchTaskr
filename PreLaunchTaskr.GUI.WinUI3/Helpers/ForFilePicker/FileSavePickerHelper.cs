using Microsoft.UI.Xaml;

using System.Collections.Generic;

using Windows.Storage.Pickers;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;

public class FileSavePickerHelper : DirectorySavePickerHelper
{
    internal FileSavePickerHelper(Window window) : base(window)
    {
        savePicker = new();
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);
    }

    private readonly FileSavePicker savePicker;

    public FileSavePickerHelper AddFileTypeChoice(string fileTypeName, IList<string> extensions)
    {
        savePicker.FileTypeChoices.Add(fileTypeName, extensions);
        return this;
    }

    public override DirectoryPickerHelper SetCommitButtonText(string text)
    {
        savePicker.CommitButtonText = text;
        return this;
    }
}
