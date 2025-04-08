using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System.Numerics;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public sealed partial class FloatingActionButton : Button
{
    public FloatingActionButton() : base()
    {
        Shadow = shadow;
        Translation += new Vector3(0, 0, 32);
    }

    public double Length
    {
        get => (double) GetValue(LengthProperty);
        set
        {
            SetValue(LengthProperty, value);
            Width = value;
            Height = value;
            CornerRadius = new CornerRadius(value / 2);
        }
    }

    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        nameof(Length),
        typeof(double),
        typeof(FloatingActionButton),
        new PropertyMetadata(56));

    private static readonly ThemeShadow shadow = new();
}

