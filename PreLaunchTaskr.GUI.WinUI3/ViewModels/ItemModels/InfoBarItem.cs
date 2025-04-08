using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class InfoBarItem
{
    public InfoBarSeverity Severity { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }

    public InfoBarItem(InfoBarSeverity severity, string title, string message)
    {
        Severity = severity;
        Title = title;
        Message = message;
    }
}
