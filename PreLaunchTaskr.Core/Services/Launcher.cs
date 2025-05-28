using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Extensions;
using PreLaunchTaskr.Core.Repositories.Implementations;
using PreLaunchTaskr.Core.Repositories.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Services;

/// <summary>
/// 必须在构造前设置 Path 以指定启动器本体在哪
/// </summary>
public class Launcher
{
    public Launcher(
        string baseDirectory,
        IProgramRepository programRepository,
        IArgumentRepository argumentRepository,
        IPreLaunchTaskRepository preLaunchTaskRepository,
        IEnvironmentVariableRepository environmentVariableRepository)
    {
        this.baseDirectory = baseDirectory;
        this.programRepository = programRepository;
        this.argumentRepository = argumentRepository;
        this.preLaunchTaskRepository = preLaunchTaskRepository;
        this.environmentVariableRepository = environmentVariableRepository;
    }

    /// <summary>
    /// 用此程序或包装它的调用者所在文件夹初始化
    /// </summary>
    /// <param name="baseDirectory">此程序或包装它的调用者所在文件夹的路径</param>
    /// <returns></returns>
    public static Launcher Init(string baseDirectory)
    {
        SqliteConnection connection = new(new SqliteConnectionStringBuilder
        {
            DataSource = Path.GetFullPath(Path.Combine(baseDirectory, GlobalProperties.SettingsLocation))
        }.ToString());
        return new Launcher(
            baseDirectory,
            new ProgramRepositoryImpl(connection),
            new ArgumentRepositoryImpl(connection),
            new PreLaunchTaskRepositoryImpl(connection),
            new EnvironmentVariableRepositoryImpl(connection));
    }

    public bool Launch(int programId, string[] args)
    {
        ProgramInfo? programInfo = programRepository.GetById(programId);
        if (programInfo is null)
            return false;

        return Launch(programInfo, args, false, false);
    }

    public bool Launch(string path, string[] args)
    {
        ProgramInfo? programInfo = programRepository.GetByPath(path);
        if (programInfo is null)
            return false;

        return Launch(programInfo, args, false, false);
    }

    public bool Launch(
        int programId,
        string[] originArgs,
        bool asAdmin,
        bool waitForExit)
    {
        ProgramInfo? programInfo = programRepository.GetById(programId);
        if (programInfo is null)
            return false;

        return Launch(programInfo, originArgs, asAdmin, waitForExit);
    }

    public bool Launch(
        string path,
        string[] originArgs,
        bool asAdmin,
        bool waitForExit)
    {
        ProgramInfo? programInfo = programRepository.GetByPath(path);
        if (programInfo is null)
            return false;

        return Launch(programInfo, originArgs, asAdmin, waitForExit);
    }

    private const int TIMEOUT = 3000;  // 毫秒

    /// <summary>
    /// 启动程序，传入参数是原启动参数。本函数会根据用户设置对参数处理，
    /// </summary>
    /// <param name="programInfo">添加的程序的信息</param>
    /// <param name="originArgs">原启动参数，不包含用户附加，不剔除用户屏蔽。</param>
    /// <returns></returns>
    public bool Launch(
        ProgramInfo programInfo,
        string[] originArgs,
        bool asAdmin,
        bool waitForExit)
    {
        string symlinkPath = GlobalProperties.SymbolicLinkPath(programInfo.Path);
        if (!File.Exists(symlinkPath))
        {
            try
            {
                File.CreateSymbolicLink(symlinkPath, programInfo.Path);
            }
            catch (Exception)
            {
                return false;
            } 
        }
        ProcessStartInfo programStartInfo = new()
        {
            FileName = symlinkPath  // 启动文件名不一样的符号链接来绕过映像劫持 Debugger
        };

        // 处理参数：先屏蔽后附加，防止把自己附加的给屏蔽了

        // 对原参数剔除后先不加入 StartInfo 启动参数，要在所有参数前附加我们自己的参数，避免造成其他影响。
        // 例如：Edge 浏览器，--single-argument 后的参数不会再被空格分成多个参数。
        List<string> afterBlock = new();
        IList<BlockedArgument> blockedArguments = argumentRepository.ListEnabledBlockedArgumentsByProgram(programInfo.Id, true);
        foreach (BlockedArgument blockedArgument in blockedArguments)
        {
            if (blockedArgument.IsRegex)
            {
                Regex regex = new(blockedArgument.Argument);
                for (int i = 0; i < originArgs.Length; i++)
                {
                    if (!regex.IsMatch(originArgs[i]))
                    {
                        afterBlock.Add(originArgs[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < originArgs.Length; i++)
                {
                    if (blockedArgument.Argument != originArgs[i])
                    {
                        afterBlock.Add(originArgs[i]);
                    }
                }
            }
        }

        IList<AttachedArgument> attachedArguments = argumentRepository.ListEnabledAttachedArgumentsByProgram(programInfo.Id, true);
        foreach (AttachedArgument attachedArgument in attachedArguments)
        {
            programStartInfo.ArgumentList.Add(attachedArgument.Argument);
        }

        if (blockedArguments.Count == 0)
        {
            programStartInfo.ArgumentList.AddAll(originArgs);
        }
        else
        {
            foreach (string arg in afterBlock)
            {
                programStartInfo.ArgumentList.Add(arg);
            }
        }

        // 处理环境变量

        Dictionary<string, List<string>> keyValues = new();
        IList<EnvironmentVariable> environmentVariables = environmentVariableRepository.ListEnabledByProgram(programInfo.Id, true);
        foreach (EnvironmentVariable environmentVariable in environmentVariables)
        {
            if (keyValues.ContainsKey(environmentVariable.Key))
            {
                keyValues[environmentVariable.Key].Add(environmentVariable.Value);
            }
            else
            {
                keyValues.Add(environmentVariable.Key, new List<string> { environmentVariable.Value });
            }
        }
        foreach (string key in keyValues.Keys)
        {
            programStartInfo.Environment.Add(key, new StringBuilder().AppendJoin(';', keyValues[key]).ToString());
        }

        // 执行启动前任务

        IList<PreLaunchTask> preLaunchTasks = preLaunchTaskRepository.ListEnabledByProgram(programInfo.Id, true);
        foreach (PreLaunchTask task in preLaunchTasks)
        {
            Process taskProcess = new();
            taskProcess.StartInfo.FileName = task.TaskPath;
            if (task.AcceptProgramArgs)
            {
                taskProcess.StartInfo.ArgumentList.AddAll(originArgs);
                if (task.IncludeAttachedArgs)
                {
                    foreach (AttachedArgument argument in attachedArguments)
                    {
                        taskProcess.StartInfo.ArgumentList.Add(argument.Argument);
                    }
                }
            }
            try
            {
                if (!taskProcess.Start())
                    continue;

                taskProcess.WaitForExit();
            }
            catch (Exception)
            {
                continue;
            }
        }

        // 带上处理后的启动参数、专属环境变量，去启动程序

        if (!asAdmin)
        {
            bool success = true;
            try
            {
                Process programProcess = new()
                {
                    StartInfo = programStartInfo,
                };

                if (!programProcess.Start())
                {
                    success = false;
                }
                else  // 10 秒内不异常退出就当它正常运行了
                {
                    programProcess.WaitForExit(waitForExit ? -1 : TIMEOUT);  // 毫秒，-1 表示无限期等待，等效于 WaitForExit()
                }
                success = !programProcess.HasExited || programProcess.ExitCode == 0;
            }
            catch  // 遇到异常后尝试以管理员身份运行
            {
                success = false;
            }
            if (success)
            {
                return true;
            }
        }

        Process programProcessAdmin = new()
        {
            StartInfo = programStartInfo,
        };
        programProcessAdmin.StartInfo.UseShellExecute = true;
        programProcessAdmin.StartInfo.Verb = "runas";

        if (!programProcessAdmin.Start())
            return false;

        programProcessAdmin.WaitForExit(waitForExit ? -1 : TIMEOUT);

        return !programProcessAdmin.HasExited || programProcessAdmin.ExitCode == 0;
    }

    /// <summary>
    /// 命令行参数：-id <id>
    /// </summary>
    /// <param name="id">需要启动的程序ID</param>
    /// <returns>命令行参数（开头有空格，结尾没空格）</returns>
    public static string CommandLineArgumentWithId(int id) => $" --{nameof(id)} {id}";

    /// <summary>
    /// 命令行参数：-baseDirectory <Disk:\path\to\program.exe>
    /// <br/>
    /// 通过指定ID查找速度更快
    /// </summary>
    /// <param name="path">需要启动的程序路径</param>
    /// <returns>命令行参数（开头有空格，结尾没空格）</returns>
    public static string CommandLineArgumentWithPath(string path) => $" --{nameof(path)} path";

    private readonly string baseDirectory;

    private readonly IProgramRepository programRepository;
    private readonly IArgumentRepository argumentRepository;
    private readonly IPreLaunchTaskRepository preLaunchTaskRepository;
    private readonly IEnvironmentVariableRepository environmentVariableRepository;
}
