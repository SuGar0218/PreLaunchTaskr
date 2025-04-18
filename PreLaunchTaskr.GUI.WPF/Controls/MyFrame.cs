using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Controls;

public class MyFrame : Frame
{
    public MyFrame() : base()
    {
        JournalOwnership = System.Windows.Navigation.JournalOwnership.OwnsJournal;
    }

    public bool IsNavigationStackEnabled
    {
        get => (bool) GetValue(IsNavigationStackEnabledProperty);
        set => SetValue(IsNavigationStackEnabledProperty, value);
    }

    public static readonly DependencyProperty IsNavigationStackEnabledProperty = DependencyProperty.Register(
        nameof(IsNavigationStackEnabled),
        typeof(bool),
        typeof(MyFrame),
        new PropertyMetadata(true, static (d, e) =>
        {
            if (!(bool) e.NewValue)
            {
                Frame self = (Frame) d;
                self.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
                self.Navigated += (o, e) => ((Frame) o).RemoveBackEntry();
            }
        }));
}
