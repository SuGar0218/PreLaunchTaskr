using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PreLaunchTaskr.GUI.WPF.ViewModels;

public class MainViewModel
{
    public ObservableCollection<ProgramListItem> Programs { get; } = new ObservableCollection<ProgramListItem>
    {
        new ProgramListItem(
            new BitmapImage(new Uri(Path.Combine(App.BaseDirectory, @"Assets\exe.png"))),
            "chrome.exe",
            "a/very/very/very/long/path/to/chrome.exe",
            false),

        new ProgramListItem(
            new BitmapImage(new Uri(Path.Combine(App.BaseDirectory, @"Assets\exe.png"))),
            "edge.exe",
            "path/to/edge.exe",
            false),
    };
}
