using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System.Collections.Generic;

using Windows.Foundation;
using Windows.Graphics;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

public class TitleBarPassthroughHelper
{
    public static TitleBarPassthroughHelper For(Window window)
    {
        if (!cacheForWindow.TryGetValue(window, out TitleBarPassthroughHelper? instance))
        {
            instance = new TitleBarPassthroughHelper(window);
            cacheForWindow.Add(window, instance);
        }
        return instance;
    }

    private static readonly Dictionary<Window, TitleBarPassthroughHelper> cacheForWindow = new();

    private TitleBarPassthroughHelper(Window window)
    {
        this.window = window;
        // System.Runtime.InteropServices.COMException
        // The WinUI Desktop Window object has already been closed.
        // this.scale = window.Content.XamlRoot.RasterizationScale;
    }

    private readonly Window window;
    private double scale;

    public void Passthrough(TabView tabView)
    {
        if (tabView.TabItems.Count == 0)
            return;

        scale = window.Content.XamlRoot.RasterizationScale;
        double passthroughWidth = 0.0;
        for (int i = 0; i < tabView.TabItems.Count; i++)
        {
            FrameworkElement? tab = (FrameworkElement) tabView.ContainerFromIndex(i) ?? tabView.TabItems[0] as FrameworkElement;
            if (tab is null)
                continue;
            passthroughWidth += tab.ActualWidth;
        }
        if (passthroughWidth == 0.0)
            return;
        if (tabView.IsAddTabButtonVisible)
        {
            passthroughWidth = passthroughWidth
                + (double) Application.Current.Resources["TabViewItemAddButtonWidth"]
                + ((Thickness) Application.Current.Resources["TabViewItemAddButtonContainerPadding"]).Left
                + ((Thickness) Application.Current.Resources["TabViewItemAddButtonContainerPadding"]).Right;
        }
        FrameworkElement firstTab = (FrameworkElement) tabView.ContainerFromIndex(0);
        Point position = firstTab.TransformToVisual(window.Content).TransformPoint(new());
        Rect rect = tabView.TransformToVisual(null).TransformBounds(new Rect(
            x: position.X,
            y: position.Y,
            width: passthroughWidth,
            height: firstTab.ActualHeight
        ));
        InputNonClientPointerSource
            .GetForWindowId(window.AppWindow.Id)
            .SetRegionRects(NonClientRegionKind.Passthrough, [GetPixelRectInt32(rect, scale)]);
    }

    public void Passthrough(FrameworkElement element)
    {
        this.scale = window.Content.XamlRoot.RasterizationScale;
        InputNonClientPointerSource
            .GetForWindowId(window.AppWindow.Id)
            .SetRegionRects(NonClientRegionKind.Passthrough, [GetUIElementPixelRectInt32(element)]);
    }

    public void Passthrough(IList<FrameworkElement> elements)
    {
        scale = window.Content.XamlRoot.RasterizationScale;
        RectInt32[] rects = new RectInt32[elements.Count];
        int count = 0;
        foreach (var element in elements)
        {
            rects[count] = GetUIElementPixelRectInt32(element);
            count++;
        }
        InputNonClientPointerSource
            .GetForWindowId(window.AppWindow.Id)
            .SetRegionRects(NonClientRegionKind.Passthrough, rects);
    }

    public void ResetPassthrough()
    {
        InputNonClientPointerSource
            .GetForWindowId(window.AppWindow.Id)
            .ClearRegionRects(NonClientRegionKind.Passthrough);
    }

    private RectInt32 GetUIElementPixelRectInt32(FrameworkElement element)
    {
        Point dipPosition = element.TransformToVisual(window.Content).TransformPoint(new Point());
        //Rect dipRect = element.TransformToVisual(null).TransformBounds(new Rect(
        //    x: dipPosition.X,
        //    y: dipPosition.Y,
        //    width: element.ActualWidth,
        //    height: element.ActualHeight
        //));
        Rect dipRect = new(
            x: dipPosition.X,
            y: dipPosition.Y,
            width: element.ActualWidth,
            height: element.ActualHeight
        );
        return GetPixelRectInt32(dipRect, scale);
    }

    private static RectInt32 GetPixelRectInt32(Rect dipRect, double scale)
    => new(
        _X: (int) (dipRect.X * scale),
        _Y: (int) (dipRect.Y * scale),
        _Width: (int) (dipRect.Width * scale),
        _Height: (int) (dipRect.Height * scale)
    );
}
