using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace PreLaunchTaskr.GUI.WPF;

public partial class App : Application
{
    internal static string BaseDirectory { get; } = Path.GetFullPath(".")!;
}
