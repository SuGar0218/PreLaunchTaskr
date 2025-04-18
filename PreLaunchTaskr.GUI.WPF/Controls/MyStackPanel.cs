using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Controls;

public class MyStackPanel : StackPanel
{
    public MyStackPanel() : base()
    {
        isFirstLoaded = true;
        needRefreshLayout = true;
        Loaded += MyStackPanel_Loaded;
    }

    private void MyStackPanel_Loaded(object sender, RoutedEventArgs e)
    {
        if (isFirstLoaded)
        {
            isFirstLoaded = false;
            foreach (UIElement element in Children)
            {
                if (element is FrameworkElement frameworkElement)
                {
                    originMargins[frameworkElement] = frameworkElement.Margin;
                }
            }
        }
        if (needRefreshLayout)
        {
            needRefreshLayout = false;
            RefreshLayout();
        }
    }

    public void RefreshLayout()
    {
        if (Orientation == Orientation.Vertical)
        {
            foreach (UIElement element in Children)
            {
                if (element is FrameworkElement frameworkElement)
                {
                    frameworkElement.HorizontalAlignment = ContentHorizontalAlignment;
                    frameworkElement.VerticalAlignment = ContentVerticalAlignment;

                    Thickness margin = originMargins[frameworkElement];
                    margin.Top += Spacing;
                    frameworkElement.Margin = margin;
                }
            }
            if (Children[0] is FrameworkElement firstFrameworkElement)
            {
                firstFrameworkElement.Margin = originMargins[firstFrameworkElement];
            }
        }
        else  // if (Orientation == Orientation.Horizontal)
        {
            foreach (UIElement element in Children)
            {
                if (element is FrameworkElement frameworkElement)
                {
                    frameworkElement.HorizontalAlignment = ContentHorizontalAlignment;
                    frameworkElement.VerticalAlignment = ContentVerticalAlignment;

                    Thickness margin = originMargins[frameworkElement];
                    margin.Left += Spacing;
                    frameworkElement.Margin = margin;
                }
            }
            if (Children[0] is FrameworkElement firstFrameworkElement)
            {
                firstFrameworkElement.Margin = originMargins[firstFrameworkElement];
            }
        }
    }

    public double Spacing
    {
        get => (double) GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public HorizontalAlignment ContentHorizontalAlignment
    {
        get => (HorizontalAlignment) GetValue(ContentHorizontalAlignmentProperty);
        set => SetValue(ContentHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment ContentVerticalAlignment
    {
        get => (VerticalAlignment) GetValue(ContentVerticalAlignmentProperty);
        set => SetValue(ContentVerticalAlignmentProperty, value);
    }

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing),
        typeof(double),
        typeof(MyStackPanel),
        new PropertyMetadata(0.0, static (d, e) =>
        {
            MyStackPanel self = (MyStackPanel) d;
            self.needRefreshLayout = true;
        }));

    public static readonly DependencyProperty ContentHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(HorizontalAlignment),
        typeof(MyStackPanel),
        new PropertyMetadata(default(HorizontalAlignment), static (d, e) =>
        {
            MyStackPanel self = (MyStackPanel) d;
            self.needRefreshLayout = true;
        }));

    public static readonly DependencyProperty ContentVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentVerticalAlignment),
        typeof(VerticalAlignment),
        typeof(MyStackPanel),
        new PropertyMetadata(default(VerticalAlignment), static (d, e) =>
        {
            MyStackPanel self = (MyStackPanel) d;
            self.needRefreshLayout = true;
        }));

    private bool isFirstLoaded;
    private bool needRefreshLayout;
    private readonly Dictionary<FrameworkElement, Thickness> originMargins = new();
}
