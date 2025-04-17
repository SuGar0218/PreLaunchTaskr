namespace PreLaunchTaskr.Configurator.NET6;

using PreLaunchTaskr.Core;
using PreLaunchTaskr.Core.Services;

using System.CommandLine;

internal partial class Program
{
    static async Task<int> Main(string[] csArgs)
    {
        if (csArgs.Length == 0)
            await Main(new string[] { "-h" });

        string[] envArgs = Environment.GetCommandLineArgs();
        return await rootCommand.InvokeAsync(csArgs);
    }

    static readonly Configurator configurator = Configurator.Init(GlobalProperties.SettingsLocation, GlobalProperties.LauncherNet6Location);
}
