using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public partial class MyButton : Button
{
    public MyButton() : base()
    {
        Shadow = shadow;
        PointerEntered += MyButton_PointerEntered;
        PointerExited += MyButton_PointerExited;
    }

    private void MyButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        Translation += new System.Numerics.Vector3(0, 0, 32);
    }

    private void MyButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        Translation -= new System.Numerics.Vector3(0, 0, 32);
    }

    private static readonly ThemeShadow shadow = new();
}
