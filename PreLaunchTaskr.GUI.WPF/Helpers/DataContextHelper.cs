using System.Windows;

namespace PreLaunchTaskr.GUI.WPF.Helpers;

public class DataContextHelper
{
    public static T GetDataContext<T>(object frameworkElement) => (T) ((FrameworkElement) frameworkElement).DataContext;
}
