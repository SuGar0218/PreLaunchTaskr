using Microsoft.UI.Xaml;

using Windows.Storage.Pickers;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;

public abstract class PathOpenPickerHelper : DirectoryPickerHelper
{
    protected PathOpenPickerHelper(Window window) : base(window) { }

    public abstract PathOpenPickerHelper SetViewMode(PickerViewMode viewMode);
    public abstract PathOpenPickerHelper AddFileTypeFilter(string fileExtension);
    //public abstract PathOpenPickerHelper SuggestStartLocation(string location);
}
