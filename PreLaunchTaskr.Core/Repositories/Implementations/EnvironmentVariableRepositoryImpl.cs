using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Common.Utils;
using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Implementations;

public class EnvironmentVariableRepositoryImpl : IEnvironmentVariableRepository
{
    public EnvironmentVariableRepositoryImpl(SqliteConnection connection) : this(
        new EnvironmentVariableDao(connection),
        new ProgramInfoDao(connection))
    {
    }

    public EnvironmentVariableRepositoryImpl(
        EnvironmentVariableDao environmentVariableDao,
        ProgramInfoDao programInfoDao)
    {
        this.environmentVariableDao = environmentVariableDao;
        this.programInfoDao = programInfoDao;
    }

    public EnvironmentVariable? Add(EnvironmentVariable variable)
    {
        int id = environmentVariableDao.Add(variable);
        if (id == -1)
            return null;
        variable.Id = id;
        return variable;
    }

    public int ClearForProgram(int programId)
    {
        return environmentVariableDao.ClearByForeignKey(programId);
    }

    public int ClearForProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ClearForProgram(programInfo.Id);
    }

    public int ClearForProgram(ProgramInfo programInfo)
    {
        return ClearForProgram(programInfo.Id);
    }

    public EnvironmentVariable? GetById(int id)
    {
        return environmentVariableDao.GetByPrimaryKey(id);
    }

    public EnvironmentVariable? GetByPath(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? null : GetById(programInfo.Id);
    }

    public List<EnvironmentVariable> List(int length = -1, int skip = 0)
    {
        return environmentVariableDao.List(length, skip);
    }

    public List<EnvironmentVariable> ListByProgram(int programId, int length = -1, int skip = 0)
    {
        return environmentVariableDao.ListByForeignKey(programId, length, skip);
    }

    public List<EnvironmentVariable> ListByProgram(string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<EnvironmentVariable>() : ListByProgram(programInfo.Id);
    }

    public List<EnvironmentVariable> ListByProgram(ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return ListByProgram(programInfo.Id);
    }

    public bool RemoveById(int id)
    {
        return environmentVariableDao.RemoveByPrimaryKey(id) > 0;
    }

    public bool Update(EnvironmentVariable variable)
    {
        return environmentVariableDao.Update(variable) > 0;
    }

    public List<EnvironmentVariable> ListEnabledByProgram(int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return environmentVariableDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public List<EnvironmentVariable> ListEnabledByProgram(string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<EnvironmentVariable>() : ListEnabledByProgram(programInfo.Id, enabled, length, skip);
    }

    public List<EnvironmentVariable> ListEnabledByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ListEnabledByProgram(programInfo.Id, enabled, length, skip);
    }

    public int ForEach(Action<EnvironmentVariable> action, int length = -1, int skip = 0)
    {
        return environmentVariableDao.ForEach(action, length, skip);
    }

    public int ForEachByProgram(Action<EnvironmentVariable> action, int programId, int length = -1, int skip = 0)
    {
        return environmentVariableDao.ForEachByForeignKey(action, programId, length, skip);
    }

    public int ForEachByProgram(Action<EnvironmentVariable> action, string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachByProgram(action, programInfo, length, skip);
    }

    public int ForEachByProgram(Action<EnvironmentVariable> action, ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return ForEachByProgram(action, programInfo.Id, length, skip);
    }

    public int ForEachEnabledByProgram(Action<EnvironmentVariable> action, int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return environmentVariableDao.ForEachEnabledByForeignKey(action, programId, enabled, length, skip);
    }

    public int ForEachEnabledByProgram(Action<EnvironmentVariable> action, string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachEnabledByProgram(action, programInfo, enabled, length, skip);
    }

    public int ForEachEnabledByProgram(Action<EnvironmentVariable> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ForEachEnabledByProgram(action, programInfo.Id, enabled, length, skip);
    }

    private readonly EnvironmentVariableDao environmentVariableDao;
    private readonly ProgramInfoDao programInfoDao;
}
