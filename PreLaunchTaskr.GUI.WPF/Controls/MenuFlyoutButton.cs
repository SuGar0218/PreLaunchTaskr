using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Controls;

public class MenuFlyoutButton : Button
{
    public MenuFlyoutButton() : base()
    {
        Loaded += MenuFlyoutButton_Loaded;
        Click += MenuFlyoutButton_Click;
    }

    private void MenuFlyoutButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (ContextMenu is not null)
        {
            ContextMenu.PlacementTarget = this;
        }
    }

    private void MenuFlyoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (ContextMenu is not null)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
