using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Common.Utils;
using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

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

    public IList<EnvironmentVariable> List(int length, int skip = 0)
    {
        return environmentVariableDao.List(length, skip);
    }

    public IList<EnvironmentVariable> ListAll()
    {
        return environmentVariableDao.List();
    }

    public IList<EnvironmentVariable> ListByProgram(int programId)
    {
        return environmentVariableDao.ListByForeignKey(programId);
    }

    public IList<EnvironmentVariable> ListByProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<EnvironmentVariable>() : ListByProgram(programInfo.Id);
    }

    public IList<EnvironmentVariable> ListByProgram(ProgramInfo programInfo)
    {
        return ListByProgram(programInfo.Id);
    }

    public IList<EnvironmentVariable> ListByProgram(int programId, int length, int skip = 0)
    {
        return environmentVariableDao.ListByForeignKey(programId, length, skip);
    }

    public IList<EnvironmentVariable> ListByProgram(string path, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<EnvironmentVariable>() : ListByProgram(programInfo.Id);
    }

    public IList<EnvironmentVariable> ListByProgram(ProgramInfo programInfo, int length, int skip = 0)
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

    public IList<EnvironmentVariable> ListEnabledByProgram(int programId, bool enabled)
    {
        return environmentVariableDao.ListEnabledByForeignKey(programId, enabled);
    }

    public IList<EnvironmentVariable> ListEnabledByProgram(string path, bool enabled)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<EnvironmentVariable>() : ListEnabledByProgram(programInfo.Id, enabled);
    }

    public IList<EnvironmentVariable> ListEnabledByProgram(ProgramInfo programInfo, bool enabled)
    {
        return ListEnabledByProgram(programInfo.Id, enabled);
    }

    public IList<EnvironmentVariable> ListEnabledByProgram(int programId, bool enabled, int length, int skip = 0)
    {
        return environmentVariableDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public IList<EnvironmentVariable> ListEnabledByProgram(string path, bool enabled, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<EnvironmentVariable>() : ListEnabledByProgram(programInfo.Id, enabled, length, skip);
    }

    public IList<EnvironmentVariable> ListEnabledByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0)
    {
        return ListEnabledByProgram(programInfo.Id, enabled, length, skip);
    }

    private readonly EnvironmentVariableDao environmentVariableDao;
    private readonly ProgramInfoDao programInfoDao;
}
