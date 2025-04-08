using PreLaunchTaskr.Configurator.NET8.Helpers;
using PreLaunchTaskr.Core.Entities;

using System.Security;

namespace PreLaunchTaskr.Configurator.NET8;

internal partial class Program
{
    private static void Show(ProgramInfo programInfo)
    {
        Console.WriteLine($"{programInfo.Id}\t{programInfo.Enabled}\t{programInfo.Path}");
    }

    #region 控制台输出提示

    private static void AlertProgramNotExists()
    {
        Console.WriteLine("没有添加过这个的程序。");
    }

    private static void AlertEmpty()
    {
        Console.WriteLine("无");
    }

    #endregion

    #region list

    private static void ListPrograms()
    {
        foreach (ProgramInfo programInfo in configurator.ListPrograms())
        {
            Show(programInfo);
        }
    }

    private static void ListAttachedArguments(ProgramInfo programInfo)
    {
        Console.WriteLine("位于");
        Console.WriteLine(programInfo.Path);
        Console.WriteLine("的程序附加了以下参数：");
        IList<AttachedArgument> list = configurator.ListAttachedArgumentsForProgram(programInfo.Id);
        if (list.Count == 0)
        {
            AlertEmpty();
            return;
        }
        Console.WriteLine($"ID\t启用\t参数");
        foreach (AttachedArgument argument in list)
        {
            Console.WriteLine($"{argument.Id}\t{argument.Enabled}\t{argument.Argument}");
        }
    }

    private static void ListAttachedArguments(int[] programId)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            ProgramInfo? programInfo = configurator.GetProgramInfo(programId[i]);
            if (programInfo is null)
            {
                AlertProgramNotExists();
                return;
            }
            Console.WriteLine();
            ListAttachedArguments(programInfo);
        }
    }

    private static void ListBlockedArguments(ProgramInfo programInfo)
    {
        Console.WriteLine("位于");
        Console.WriteLine(programInfo.Path);
        Console.WriteLine("的程序屏蔽了以下参数：");
        IList<BlockedArgument> list = configurator.ListBlockedArgumentsForProgram(programInfo.Id);
        if (list.Count == 0)
        {
            AlertEmpty();
            return;
        }
        Console.WriteLine($"ID\t启用\t正则表达式\t参数");
        foreach (BlockedArgument argument in list)
        {
            Console.WriteLine($"{argument.Id}\t{argument.Enabled}\t{argument.IsRegex}\t{argument.Argument}");
        }
    }

    private static void ListBlockedArguments(int[] programId)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            ProgramInfo? programInfo = configurator.GetProgramInfo(programId[i]);
            if (programInfo is null)
            {
                AlertProgramNotExists();
                return;
            }
            Console.WriteLine();
            ListBlockedArguments(programInfo);
        }
    }

    private static void ListPreLaunchTasks(int[] programId)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            IList<PreLaunchTask> list = configurator.ListPreLaunchTaskForProgram(programId[i]);
            if (list.Count == 0)
            {
                AlertEmpty();
                return;
            }
            Console.WriteLine("位于");
            Console.WriteLine(list.First().ProgramInfo.Path);
            Console.WriteLine("的程序启动前执行以下任务：");
            Console.WriteLine($"ID\t启用\t批处理脚本");
            foreach (PreLaunchTask item in list)
            {
                Console.WriteLine($"{item.Id}\t{item.Enabled}\t{item.TaskPath}");
            }
        }
    }

    private static void ListEnvironmentVariables(int[] programId)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            IList<EnvironmentVariable> list = configurator.ListEnvironmentVariablesForProgram(programId[i]);
            if (list.Count == 0)
            {
                AlertEmpty();
                return;
            }
            Console.WriteLine("位于");
            Console.WriteLine(list.First().ProgramInfo.Path);
            Console.WriteLine("的程序有以下专属环境变量：");
            Console.WriteLine($"ID\t启用\t键\t值");
            foreach (EnvironmentVariable item in list)
            {
                Console.WriteLine($"{item.Id}\t{item.Enabled}\t{item.Key}\t{item.Value}");
            }
        }
    }

    #endregion

    #region add

    private static void AddProgram(string[] path, bool enable)
    {
        for (int i = 0; i < path.Length; i++)
        {
            configurator.AddProgram(new ProgramInfo(path[i], enable));
        }
    }

    private static void AttachArgument(int[] programId, string[] argument, bool enable)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            ProgramInfo? programInfo = configurator.GetProgramInfo(programId[i]);
            if (programInfo is null)
            {
                AlertProgramNotExists();
                return;
            }
            for (int j = 0; j < argument.Length; j++)
            {
                configurator.AttachArgument(new AttachedArgument(programInfo, argument[j], enable));
            }
        }
    }

    private static void BlockArgument(int[] programId, string[] argument, bool enable, bool isRegex)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            ProgramInfo? programInfo = configurator.GetProgramInfo(programId[i]);
            if (programInfo is null)
            {
                AlertProgramNotExists();
                return;
            }
            for (int j = 0; j < argument.Length; j++)
            {
                configurator.BlockArgument(new BlockedArgument(programInfo, argument[j], enable, isRegex));
            }
        }
    }

    private static void AddPreLaunchTask(
        int[] programId,
        string[] taskPath,
        bool acceptProgramArgs,
        bool includeAttachedArgs,
        bool enable)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            ProgramInfo? programInfo = configurator.GetProgramInfo(programId[i]);
            if (programInfo is null)
            {
                AlertProgramNotExists();
                return;
            }
            for (int j = 0; j < taskPath.Length; j++)
            {
                configurator.AddPreLaunchTask(new PreLaunchTask(
                    programInfo,
                    taskPath[j],
                    acceptProgramArgs,
                    includeAttachedArgs,
                    enable));
            }
        }
    }
    
    private static void AddEnvironmentVariable(int[] programId, string key, string[] value, bool enable)
    {
        for (int i = 0; i < programId.Length; i++)
        {
            ProgramInfo? programInfo = configurator.GetProgramInfo(programId[i]);
            if (programInfo is null)
            {
                AlertProgramNotExists();
                return;
            }
            for (int j = 0; j < value.Length; j++)
            {
                configurator.AddEnvironmentVariable(new EnvironmentVariable(programInfo, key, value[j], enable));
            }
        }
    }

    #endregion

    #region remove

    private static void RemoveProgram(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.RemoveProgram(id[i]);
        }
    }

    private static void RemoveProgram(string[] path)
    {
        for (int i = 0; i < path.Length; i++)
        {
            configurator.RemoveProgram(path[i]);
        }
    }

    private static void RemoveAttachedArgument(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.RemoveAttachedArgument(id[i]);
        }
    }

    private static void RemoveBlockedArgument(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.RemoveBlockedArgument(id[i]);
        }
    }

    private static void RemovePreLaunchTask(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.RemovePreLaunchTask(id[i]);
        }
    }

    private static void RemoveEnvironmentVariable(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.RemoveEnvironmentVariable(id[i]);
        }
    }

    #endregion

    #region enable

    private static void EnableProgram(int[] id)
    {
        try
        {
            for (int i = 0; i < id.Length; i++)
            {
                if (!configurator.EnableProgram(id[i]))
                {
                    ConsoleHelper.PrintWarning("未添加此程序");
                }
            }
        }
        catch (SecurityException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintError("需要操作注册表，需要管理员权限，请用管理员终端运行。");
            ConsoleHelper.PrintError(e.Message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    private static void EnableProgram(string[] path)
    {
        try
        {
            for (int i = 0; i < path.Length; i++)
            {
                configurator.EnableProgram(path[i]);
            }
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("需要操作注册表，需要管理员权限，请用管理员终端运行。");
            Console.WriteLine(e.Message);
        }
    }

    private static void EnableAttachedArgument(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.EnableAttachedArgument(id[i]);
        }
    }

    private static void EnableBlockedArgument(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
            configurator.EnableBlockedArgument(id[i]);
    }

    private static void EnablePreLaunchTask(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.EnablePreLaunchTask(id[i]);
        }
    }

    private static void EnableEnvironmentVariable(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.EnableEnvironmentVariable(id[i]);
        }
    }

    #endregion

    #region disable

    private static void DisableProgram(int[] id)
    {
        try
        {
            for (int i = 0; i < id.Length; i++)
            {
                configurator.EnableProgram(id[i], false);
            }
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("需要操作注册表，需要管理员权限，请用管理员终端运行。");
            Console.WriteLine(e.Message);
        }
    }

    private static void DisableProgram(string[] path)
    {
        try
        {
            for (int i = 0; i < path.Length; i++)
            {
                configurator.EnableProgram(path[i], false);
            }
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("需要操作注册表，需要管理员权限，请用管理员终端运行。");
            Console.WriteLine(e.Message);
        }
    }

    private static void DisableAttachedArgument(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.EnableAttachedArgument(id[i], false);
        }
    }

    private static void DisableBlockedArgument(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            configurator.EnableBlockedArgument(id[i], false);
        }
    }

    private static void DisablePreLaunchTask(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
            configurator.EnablePreLaunchTask(id[i], false);
    }

    private static void DisableEnvironmentVariable(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
            configurator.EnableEnvironmentVariable(id[i], false);
    }

    #endregion

    private static void RunSilent(bool silent)
    {
        if (silent)
            Windows.Win32.PInvoke.FreeConsole();

        Console.WriteLine("此程序没有在静默模式下运行");
    }
}
