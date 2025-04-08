using Microsoft.UI.Xaml;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;

public abstract class DirectoryPickerHelper
{
    public static FolderPickerHelper OpenFolderForWindow(Window window) => new(window);
    public static FileOpenPickerHelper OpenFileForWindow(Window window) => new(window);
    public static FileSavePickerHelper SaveFileForWindow(Window window) => new(window);

    public abstract DirectoryPickerHelper SetCommitButtonText(string text);

    protected DirectoryPickerHelper(Window window)
    {
        hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
    }

    protected readonly nint hWnd;
}
