using Microsoft.UI.Xaml;

using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;

public class FileOpenPickerHelper : DirectoryOpenPickerHelper
{
    internal FileOpenPickerHelper(Window window) : base(window)
    {
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
    }

    private readonly FileOpenPicker picker = new();

    public async Task<StorageFile?> PickSingleAsync()
    {
        if (picker.FileTypeFilter.Count == 0)
            picker.FileTypeFilter.Add("*");

        return await picker.PickSingleFileAsync();
    }

    public override FileOpenPickerHelper SetViewMode(PickerViewMode viewMode)
    {
        picker.ViewMode = viewMode;
        return this;
    }

    public override FileOpenPickerHelper AddFileTypeFilter(string fileExtension)
    {
        picker.FileTypeFilter.Add(fileExtension.StartsWith('.') ? fileExtension : '.' + fileExtension);
        return this;
    }

    public override FileOpenPickerHelper SetCommitButtonText(string text)
    {
        picker.CommitButtonText = text;
        return this;
    }
}
