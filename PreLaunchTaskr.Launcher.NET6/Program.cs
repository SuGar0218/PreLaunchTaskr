namespace PreLaunchTaskr.Launcher.NET6;

/// <summary>
/// 此程序可能因为映像劫持而运行
/// <br/>
/// 传入的参数：被劫持的程序名、原本用来启动被劫持的程序的参数
/// </summary>
internal partial class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
            return 0;

        args = Environment.GetCommandLineArgs();
        string baseUri = Path.GetDirectoryName(args[0])!;
        string programPath = args[1];
        string[] originArgs = args[2..];
        Core.Services.Launcher launcher = Core.Services.Launcher.Init(baseUri);

        return await launcher.Launch(programPath, originArgs) ? 0 : -1;
    }
}
