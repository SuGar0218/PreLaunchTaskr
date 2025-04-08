using Microsoft.UI.Xaml;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

public class DataContextHelper
{
    public static T GetDataContext<T>(object frameworkElement) => (T) ((FrameworkElement) frameworkElement).DataContext;
}
