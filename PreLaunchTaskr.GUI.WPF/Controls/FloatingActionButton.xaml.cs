// WPF

using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Controls;

public sealed partial class FloatingActionButton : Button
{
    public FloatingActionButton() : base()
    {
    }

    //static FloatingActionButton()
    //{
    //    RoundCornerBorderStyle = new Style
    //    {
    //        TargetType = typeof(Border)
    //    };
    //    BorderCornerRadiusSetter = new Setter
    //    {
    //        Property = Border.CornerRadiusProperty,
    //        Value = new CornerRadius(0)
    //    };
    //    RoundCornerBorderStyle.Setters.Add(BorderCornerRadiusSetter);
    //}

    public double Length
    {
        get => (double) GetValue(LengthProperty);
        set
        {
            SetValue(LengthProperty, value);
        }
    }

    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        nameof(Length),
        typeof(double),
        typeof(FloatingActionButton),
        new PropertyMetadata(0.0, static (d, e) =>
        {
            FloatingActionButton self = (FloatingActionButton) d;
            double length = (double) e.NewValue;
            self.Width = length;
            self.Height = length;
            //BorderCornerRadiusSetter!.Value = new CornerRadius(length / 2);
        }));

    //private static readonly Style RoundCornerBorderStyle;
    //private static readonly Setter BorderCornerRadiusSetter;
}

