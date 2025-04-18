// WPF
using System.Windows;
using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.Controls;

public class MyGrid : Grid
{
    public MyGrid() : base()
    {
        isFirstLoaded = true;
        needRefreshLayout = true;
        Loaded += MyGrid_Loaded;
    }

    private void MyGrid_Loaded(object sender, RoutedEventArgs e)
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
        foreach (UIElement element in Children)
        {
            if (element is FrameworkElement frameworkElement)
            {
                frameworkElement.HorizontalAlignment = ContentHorizontalAlignment;
                frameworkElement.VerticalAlignment = ContentVerticalAlignment;

                if (GetRow(element) > 0 && RowSpacing != 0)
                {
                    Thickness margin = originMargins[frameworkElement];
                    margin.Top += RowSpacing;
                    frameworkElement.Margin = margin;
                }
                if (GetColumn(element) > 0 && ColumnSpacing != 0)
                {
                    Thickness margin = originMargins[frameworkElement];
                    margin.Left += ColumnSpacing;
                    frameworkElement.Margin = margin;
                }
            }
        }
    }

    public double RowSpacing
    {
        get => (double) GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    public double ColumnSpacing
    {
        get => (double) GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
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

    public static readonly DependencyProperty RowSpacingProperty = DependencyProperty.Register(
        nameof(RowSpacing),
        typeof(double),
        typeof(MyGrid),
        new PropertyMetadata(0.0, static (d, e) =>
        {
            MyGrid self = (MyGrid) d;
            self.needRefreshLayout = true;
        }));

    public static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register(
        nameof(ColumnSpacing),
        typeof(double),
        typeof(MyGrid),
        new PropertyMetadata(0.0, static (d, e) =>
        {
            MyGrid self = (MyGrid) d;
            self.needRefreshLayout = true;
        }));

    public static readonly DependencyProperty ContentHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(HorizontalAlignment),
        typeof(MyGrid),
        new PropertyMetadata(default(HorizontalAlignment), static (d, e) =>
        {
            MyGrid self = (MyGrid) d;
            self.needRefreshLayout = true;
        }));

    public static readonly DependencyProperty ContentVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentVerticalAlignment),
        typeof(VerticalAlignment),
        typeof(MyGrid),
        new PropertyMetadata(default(VerticalAlignment), static (d, e) =>
        {
            MyGrid self = (MyGrid) d;
            self.needRefreshLayout = true;
        }));

    private bool isFirstLoaded;
    private bool needRefreshLayout;
    private readonly Dictionary<FrameworkElement, Thickness> originMargins = new();
}
