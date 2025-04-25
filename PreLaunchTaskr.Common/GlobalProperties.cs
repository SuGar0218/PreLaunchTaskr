namespace PreLaunchTaskr.Core;

/// <summary>
/// 此解决方案中所有程序公用的属性
/// <br/>
/// 最后把所有可执行文件和数据库文件放在同一文件夹，
/// 这里的路径写的都是相对于此文件夹的相对位置。
/// </summary>
public class GlobalProperties
{
    public const string SettingsLocation = @".\settings.sqlite";
    public const string ConfiguratorNet6Location = @".\PreLaunchTaskr.Configurator.NET6.exe";
    public const string ConfiguratorNet8Location = @".\PreLaunchTaskr.Configurator.NET8.exe";
    public const string LauncherNet6Location = @".\PreLaunchTaskr.Launcher.NET6.exe";
    public const string LauncherNet8Location = @".\PreLaunchTaskr.Launcher.NET8.exe";
    public const string WpfLocation = @".\PreLaunchTaskr.GUI.WPF.exe";
    public const string WinUI3Location = @".\PreLaunchTaskr.GUI.WinUI3.exe";

    public static string SymbolicLinkName(string filename) => "_" + filename;

    public static string SymbolicLinkPath(string path)
    {
        string? directory = Path.GetDirectoryName(path) ?? throw new ArgumentException("此路径不指向一个文件");
        string filename = Path.GetFileName(path);
        return Path.Combine(directory, SymbolicLinkName(filename));
    }
}
