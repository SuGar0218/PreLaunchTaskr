using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PreLaunchTaskr.GUI.WPF.ViewModels;

public class ProgramListItem
{
    public ImageSource Icon { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public bool Enabled { get; set; }

    public ProgramListItem(ImageSource icon, string name, string path, bool enabled)
    {
        Icon = icon;
        Name = name;
        Path = path;
        Enabled = enabled;
    }
}
