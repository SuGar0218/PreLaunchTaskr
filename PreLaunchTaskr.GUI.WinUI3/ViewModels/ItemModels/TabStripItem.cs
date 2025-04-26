using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class TabStripItem
{
    public TabStripItem(string header, Type pageType, object? extraData = null) : this(header, null, true, pageType, extraData) { }

    public TabStripItem(string header, IconSource icon, Type pageType, object? extraData = null) : this(header, icon, true, pageType, extraData) { }

    public TabStripItem(string header, IconSource? icon, bool closeable, Type pageType, object? extraData)
    {
        Header = header;
        Icon = icon;
        Closeable = closeable;
        PageType = pageType;
        ExtraData = extraData;
    }

    public string Header { get; }

    public IconSource? Icon { get; }

    public bool Closeable { get; }

    public Type PageType { get; }

    public object? ExtraData { get; }
}
