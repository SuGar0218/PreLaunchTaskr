using Microsoft.UI.Xaml;

using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;

public class FolderPickerHelper : PathOpenPickerHelper
{
    public FolderPickerHelper(Window window) : base(window)
    {
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
    }

    private readonly FolderPicker picker = new();

    public async Task<StorageFolder> PickSingleAsync() => await picker.PickSingleFolderAsync();

    public override FolderPickerHelper AddFileTypeFilter(string fileExtension)
    {
        picker.FileTypeFilter.Add(fileExtension.StartsWith('.') ? fileExtension : '.' + fileExtension);
        return this;
    }

    public override FolderPickerHelper SetCommitButtonText(string text)
    {
        picker.CommitButtonText = text;
        return this;
    }

    public override FolderPickerHelper SetViewMode(PickerViewMode viewMode)
    {
        picker.ViewMode = viewMode;
        return this;
    }
}
