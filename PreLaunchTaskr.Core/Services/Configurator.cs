using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Implementations;
using PreLaunchTaskr.Core.Repositories.Interfaces;
using PreLaunchTaskr.Core.Utils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PreLaunchTaskr.Core.Services;

/// <summary>
/// 包装了所需功能的配置器
/// <br/>
/// 对于保存和读取，配置器总是与持久化存储介质交互。请考虑缓存以避免频繁调用。
/// </summary>
public class Configurator
{
    /// <summary>
    /// 指定程序配置的设置存储的位置以初始化
    /// </summary>
    /// <param name="pathToSettings">配置文件所在位置</param>
    /// <param name="pathToLauncher">启动器所在位置</param>
    /// <returns></returns>
    public static Configurator Init(string pathToSettings, string pathToLauncher)
    {
        SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = pathToSettings }.ToString());
        ProgramInfoDao programInfoDao = new ProgramInfoDao(connection);
        AttachedArgumentDao attachedArgumentDao = new AttachedArgumentDao(connection);
        BlockedArgumentDao blockedArgumentDao = new BlockedArgumentDao(connection);
        PreLaunchTaskDao preLaunchTaskDao = new PreLaunchTaskDao(connection);
        EnvironmentVariableDao environmentVariableDao = new EnvironmentVariableDao(connection);
        return new Configurator(
            pathToLauncher,
            new ProgramRepositoryImpl(programInfoDao, attachedArgumentDao, blockedArgumentDao, preLaunchTaskDao, environmentVariableDao),
            new ArgumentRepositoryImpl(attachedArgumentDao, blockedArgumentDao, programInfoDao),
            new PreLaunchTaskRepositoryImpl(preLaunchTaskDao, programInfoDao),
            new EnvironmentVariableRepositoryImpl(environmentVariableDao, programInfoDao));
    }

    public Configurator(
        string pathToLauncher,
        IProgramRepository programRepository,
        IArgumentRepository argumentRepository,
        IPreLaunchTaskRepository preLaunchTaskRepository,
        IEnvironmentVariableRepository environmentVariableRepository)
    {
        this.pathToLauncher = pathToLauncher;
        this.programRepository = programRepository;
        this.argumentRepository = argumentRepository;
        this.preLaunchTaskRepository = preLaunchTaskRepository;
        this.environmentVariableRepository = environmentVariableRepository;
    }

    #region 程序：列出、查询、添加、更改、删除、清空

    public IList<ProgramInfo> ListPrograms()
    {
        return programRepository.ListAll();
    }

    public IList<ProgramInfo> ListPrograms(int length, int skip = 0)
    {
        return programRepository.List(length, skip);
    }

    public ProgramInfo? GetProgramInfo(int id)
    {
        return programRepository.GetById(id);
    }

    public ProgramInfo? GetProgramInfo(string path)
    {
        return programRepository.GetByPath(path);
    }

    public ProgramInfo? AddProgram(ProgramInfo programInfo)
    {
        ProgramInfo? addedProgramInfo = programRepository.Add(programInfo);
        if (addedProgramInfo is null)
            return null;

        if (programInfo.Enabled)
        {
            ImageFileExecutionOptions.SetDebugger(
                Path.GetFileName(programInfo.Path),
                $"{Path.GetFullPath(pathToLauncher)} {Launcher.CommandLineArgumentWithId(programInfo.Id)}");
        }
        return addedProgramInfo;
    }

    /// <summary>
    /// 需要操作注册表、创建符号链接，需要管理员权限。
    /// </summary>
    public bool EnableProgram(int programId, bool enable = true)
    {
        ProgramInfo? programInfo = programRepository.GetById(programId);
        return programInfo is not null && EnableProgram(programInfo, enable);
    }

    /// <summary>
    /// 需要操作注册表、创建符号链接，需要管理员权限。
    /// </summary>
    public bool EnableProgram(string path, bool enable = true)
    {
        ProgramInfo? programInfo = programRepository.GetByPath(path);
        return programInfo is not null && EnableProgram(programInfo, enable);
    }

    /// <summary>
    /// 需要操作注册表、创建符号链接，需要管理员权限。
    /// </summary>
    public bool EnableProgram(ProgramInfo programInfo, bool enable = true)
    {
        if (programInfo.Enabled == enable)
            return true;

        programInfo.Enabled = enable;
        string symlinkPath = Properties.SymbolicLinkPath(programInfo.Path);
        if (enable)
        {
            if (File.Exists(symlinkPath))
                File.Delete(symlinkPath);
            File.CreateSymbolicLink(symlinkPath, programInfo.Path);
            ImageFileExecutionOptions.SetDebugger(
                Path.GetFileName(programInfo.Path),
                $"{Path.GetFullPath(pathToLauncher)}");
        }
        else
        {
            if (File.Exists(symlinkPath))
                File.Delete(symlinkPath);
            ImageFileExecutionOptions.UnsetDebugger(Path.GetFileName(programInfo.Path));
        }
        return UpdateProgram(programInfo);
    }

    private bool UpdateProgram(ProgramInfo programInfo)
    {
        return programRepository.Update(programInfo);
    }

    public bool RemoveProgram(int id)
    {
        return programRepository.RemoveById(id);
    }

    public bool RemoveProgram(string path)
    {
        return programRepository.RemoveByPath(path);
    }

    #endregion

    #region 程序要附加或屏蔽的参数：按程序列出、添加、更改、删除、按程序清空

    public IList<AttachedArgument> ListAttachedArgumentsForProgram(int id)
    {
        return argumentRepository.ListAttachedArgumentsByProgram(id);
    }

    public IList<AttachedArgument> ListAttachedArgumentsForProgram(string path)
    {
        return argumentRepository.ListAttachedArgumentsByProgram(path);
    }

    public IList<BlockedArgument> ListBlockedArgumentsForProgram(int id)
    {
        return argumentRepository.ListBlockedArgumentsByProgram(id);
    }

    public IList<BlockedArgument> ListBlockedArgumentsForProgram(string path)
    {
        return argumentRepository.ListBlockedArgumentsByProgram(path);
    }

    public AttachedArgument? AttachArgument(AttachedArgument argument)
    {
        return argumentRepository.AddAttachedArgument(argument);
    }

    public BlockedArgument? BlockArgument(BlockedArgument argument)
    {
        return argumentRepository.AddBlockedArgument(argument);
    }

    public bool EnableAttachedArgument(int attachedArgumentId, bool enable = true)
    {
        AttachedArgument? argument = argumentRepository.GetAttachedArgumentById(attachedArgumentId);
        if (argument is null)
            return false;
        if (argument.Enabled == enable)
            return true;
        argument.Enabled = enable;
        return UpdateAttachedArgument(argument);
    }

    public bool UpdateAttachedArgument(AttachedArgument argument)
    {
        return argumentRepository.UpdateAttachedArgument(argument);
    }

    public bool EnableBlockedArgument(int blockedArgumentId, bool enable = true)
    {
        BlockedArgument? argument = argumentRepository.GetBlockedArgumentById(blockedArgumentId);
        if (argument is null)
            return false;
        if (argument.Enabled == enable)
            return true;
        argument.Enabled = enable;
        return UpdateBlockedArgument(argument);
    }

    public bool UpdateBlockedArgument(BlockedArgument argument)
    {
        return argumentRepository.UpdateBlockedArgument(argument);
    }

    public bool RemoveAttachedArgument(int id)
    {
        return argumentRepository.RemoveAttachedArgumentById(id);
    }

    public bool RemoveBlockedArgument(int id)
    {
        return argumentRepository.RemoveBlockedArgumentById(id);
    }

    public int ClearAttachedArgumentForProgram(int id)
    {
        return argumentRepository.ClearAttachedArgumentsForProgram(id);
    }

    public int ClearAttachedArgumentForProgram(string path)
    {
        return argumentRepository.ClearAttachedArgumentsForProgram(path);
    }

    public int ClearAttachedArgumentForProgram(ProgramInfo programInfo)
    {
        return argumentRepository.ClearAttachedArgumentsForProgram(programInfo);
    }

    public int ClearBlockedArgumentForProgram(int id)
    {
        return argumentRepository.ClearBlockedArgumentsForProgram(id);
    }

    public int ClearBlockedArgumentForProgram(string path)
    {
        return argumentRepository.ClearBlockedArgumentsForProgram(path);
    }

    public int ClearBlockedArgumentForProgram(ProgramInfo programInfo)
    {
        return argumentRepository.ClearBlockedArgumentsForProgram(programInfo);
    }

    #endregion

    #region 启动前任务：按程序列出、添加、更改、删除、按程序清空

    public IList<PreLaunchTask> ListPreLaunchTaskForProgram(int id)
    {
        return preLaunchTaskRepository.ListByProgram(id);
    }

    public IList<PreLaunchTask> ListPreLaunchTaskForProgram(string path)
    {
        return preLaunchTaskRepository.ListByProgram(path);
    }

    public IList<PreLaunchTask> ListPreLaunchTaskForProgram(ProgramInfo programInfo)
    {
        return preLaunchTaskRepository.ListByProgram(programInfo);
    }

    public PreLaunchTask? AddPreLaunchTask(PreLaunchTask preLaunchTask)
    {
        return preLaunchTaskRepository.Add(preLaunchTask);
    }

    public bool EnablePreLaunchTask(int preLaunchTaskId, bool enable = true)
    {
        PreLaunchTask? preLaunchTask = preLaunchTaskRepository.GetById(preLaunchTaskId);
        if (preLaunchTask is null)
            return false;
        if (preLaunchTask.Enabled == enable)
            return true;
        preLaunchTask.Enabled = enable;
        return UpdatePreLaunchTask(preLaunchTask);
    }

    public bool UpdatePreLaunchTask(PreLaunchTask preLaunchTask)
    {
        return preLaunchTaskRepository.Update(preLaunchTask);
    }

    public bool RemovePreLaunchTask(int id)
    {
        return preLaunchTaskRepository.RemoveById(id);
    }

    public int ClearPreLaunchTaskForProgram(int id)
    {
        return preLaunchTaskRepository.ClearForProgram(id);
    }

    public int ClearPreLaunchTaskForProgram(string path)
    {
        return preLaunchTaskRepository.ClearForProgram(path);
    }

    public int ClearPreLaunchTaskForProgram(ProgramInfo programInfo)
    {
        return preLaunchTaskRepository.ClearForProgram(programInfo);
    }

    #endregion

    #region 程序专属环境变量：按程序列出、添加、更改、删除、按程序清空

    public IList<EnvironmentVariable> ListEnvironmentVariablesForProgram(int id)
    {
        return environmentVariableRepository.ListByProgram(id);
    }

    public IList<EnvironmentVariable> ListEnvironmentVariablesForProgram(string path)
    {
        return environmentVariableRepository.ListByProgram(path);
    }

    public IList<EnvironmentVariable> ListEnvironmentVariablesForProgram(ProgramInfo programInfo)
    {
        return environmentVariableRepository.ListByProgram(programInfo);
    }

    public EnvironmentVariable? AddEnvironmentVariable(EnvironmentVariable variable)
    {
        return environmentVariableRepository.Add(variable);
    }

    public bool EnableEnvironmentVariable(int variableId, bool enable = true)
    {
        EnvironmentVariable? variable = environmentVariableRepository.GetById(variableId);
        if (variable is null)
            return false;
        if (variable.Enabled == enable)
            return true;
        variable.Enabled = enable;
        return UpdateEnvironmentVariable(variable);
    }

    public bool UpdateEnvironmentVariable(EnvironmentVariable variable)
    {
        return environmentVariableRepository.Update(variable);
    }

    public bool RemoveEnvironmentVariable(int id)
    {
        return environmentVariableRepository.RemoveById(id);
    }

    public int ClearEnvironmentVariablesForProgram(int id)
    {
        return environmentVariableRepository.ClearForProgram(id);
    }

    public int ClearEnvironmentVariablesForProgram(string path)
    {
        return environmentVariableRepository.ClearForProgram(path);
    }

    public int ClearEnvironmentVariablesForProgram(ProgramInfo programInfo)
    {
        return environmentVariableRepository.ClearForProgram(programInfo);
    }

    #endregion

    private readonly string pathToLauncher;

    private readonly IProgramRepository programRepository;
    private readonly IArgumentRepository argumentRepository;
    private readonly IPreLaunchTaskRepository preLaunchTaskRepository;
    private readonly IEnvironmentVariableRepository environmentVariableRepository;
}
