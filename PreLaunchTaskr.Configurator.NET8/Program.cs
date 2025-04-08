namespace PreLaunchTaskr.Configurator.NET8;

using PreLaunchTaskr.Configurator.NET8;
using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Services;

using System.CommandLine;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;

internal partial class Program
{
    static async Task<int> Main(string[] csArgs)
    {
        if (csArgs.Length == 0)
            await Main(["-h"]);

        string[] envArgs = Environment.GetCommandLineArgs();
        return await rootCommand.InvokeAsync(csArgs);
    }

    static readonly Configurator configurator = Configurator.Init(Properties.SettingsLocation, Properties.LauncherNet8Location);
}
