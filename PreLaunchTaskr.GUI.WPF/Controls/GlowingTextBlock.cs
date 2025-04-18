using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PreLaunchTaskr.GUI.WPF.Controls;

public class GlowingTextBlock : TextBlock
{
    public GlowingTextBlock() : base()
    {
        Effect = effect;
    }

    private static readonly DropShadowEffect effect = new()
    {
        Color = Colors.White,
        Opacity = 1.0,
        BlurRadius = 16,
        ShadowDepth = 0,
        RenderingBias = RenderingBias.Performance
    };
}
