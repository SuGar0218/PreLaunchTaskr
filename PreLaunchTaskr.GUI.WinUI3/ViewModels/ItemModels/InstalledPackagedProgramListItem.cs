using Microsoft.UI.Xaml.Media.Imaging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public class InstalledPackagedProgramListItem
{
    public InstalledPackagedProgramListItem(Package package)
    {
        this.package = package;
        Icon = new BitmapImage(package.Logo);
    }

    public string Name => package.DisplayName;

    public BitmapImage Icon { get; }

    public string Publisher => package.PublisherDisplayName;

    public string InstallPath => package.InstalledLocation.Path;

    private readonly Package package;

    //private static readonly BitmapImage defaultProgramIcon = new(new Uri(Path.Combine(App.BaseDirectory, @"Assets\Square150x150Logo.scale-200.png")));
}
