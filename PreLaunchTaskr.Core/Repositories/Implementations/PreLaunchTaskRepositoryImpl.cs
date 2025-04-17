using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Common.Utils;
using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Implementations;

public class PreLaunchTaskRepositoryImpl : IPreLaunchTaskRepository
{
    public PreLaunchTaskRepositoryImpl(SqliteConnection connection) : this(
        new PreLaunchTaskDao(connection),
        new ProgramInfoDao(connection))
    {
    }

    public PreLaunchTaskRepositoryImpl(
        PreLaunchTaskDao preLaunchTaskDao,
        ProgramInfoDao programInfoDao)
    {
        this.preLaunchTaskDao = preLaunchTaskDao;
        this.programInfoDao = programInfoDao;
    }

    public PreLaunchTask? Add(PreLaunchTask preLaunchTask)
    {
        int id = preLaunchTaskDao.Add(preLaunchTask);
        if (id == -1)
            return null;
        preLaunchTask.Id = id;
        return preLaunchTask;
    }

    public int ClearForProgram(int programId)
    {
        return preLaunchTaskDao.ClearByForeignKey(programId);
    }

    public int ClearForProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : preLaunchTaskDao.ClearByForeignKey(programInfo.Id);
    }

    public int ClearForProgram(ProgramInfo programInfo)
    {
        return ClearForProgram(programInfo.Id);
    }

    public PreLaunchTask? GetById(int id)
    {
        return preLaunchTaskDao.GetByPrimaryKey(id);
    }

    public IList<PreLaunchTask> List(int length, int skip = 0)
    {
        return preLaunchTaskDao.List(length, skip);
    }

    public IList<PreLaunchTask> ListAll()
    {
        return preLaunchTaskDao.List();
    }

    public IList<PreLaunchTask> ListByProgram(int programId)
    {
        return preLaunchTaskDao.ListByForeignKey(programId);
    }

    public IList<PreLaunchTask> ListByProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<PreLaunchTask>() : ListByProgram(programInfo.Id);
    }

    public IList<PreLaunchTask> ListByProgram(ProgramInfo programInfo)
    {
        return ListByProgram(programInfo.Id);
    }

    public IList<PreLaunchTask> ListByProgram(int programId, int length, int skip = 0)
    {
        return preLaunchTaskDao.ListByForeignKey(programId, length, skip);
    }

    public IList<PreLaunchTask> ListByProgram(string path, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<PreLaunchTask>() : ListByProgram(programInfo.Id, length, skip);
    }

    public IList<PreLaunchTask> ListByProgram(ProgramInfo programInfo, int length, int skip = 0)
    {
        return ListByProgram(programInfo.Id, length, skip);
    }

    public bool RemoveById(int id)
    {
        return preLaunchTaskDao.RemoveByPrimaryKey(id) > 0;
    }

    public bool Update(PreLaunchTask preLaunchTask)
    {
        return preLaunchTaskDao.Update(preLaunchTask) > 0;
    }

    public IList<PreLaunchTask> ListEnabledByProgram(int programId, bool enabled)
    {
        return preLaunchTaskDao.ListEnabledByForeignKey(programId, enabled);
    }

    public IList<PreLaunchTask> ListEnabledByProgram(string path, bool enabled)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<PreLaunchTask>() : ListByProgram(programInfo.Id);
    }

    public IList<PreLaunchTask> ListEnabledByProgram(ProgramInfo programInfo, bool enabled)
    {
        return ListEnabledByProgram(programInfo.Id, enabled);
    }

    public IList<PreLaunchTask> ListEnabledByProgram(int programId, bool enabled, int length, int skip = 0)
    {
        return preLaunchTaskDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public IList<PreLaunchTask> ListEnabledByProgram(string path, bool enabled, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<PreLaunchTask>() : ListByProgram(programInfo.Id, length, skip);
    }

    public IList<PreLaunchTask> ListEnabledByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0)
    {
        return ListEnabledByProgram(programInfo.Id, enabled, length, skip);
    }

    private readonly PreLaunchTaskDao preLaunchTaskDao;
    private readonly ProgramInfoDao programInfoDao;
}
