using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Common.Utils;
using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

using System;
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

    public List<PreLaunchTask> List(int length = -1, int skip = 0)
    {
        return preLaunchTaskDao.List(length, skip);
    }

    public List<PreLaunchTask> ListByProgram(int programId, int length = -1, int skip = 0)
    {
        return preLaunchTaskDao.ListByForeignKey(programId, length, skip);
    }

    public List<PreLaunchTask> ListByProgram(string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<PreLaunchTask>() : ListByProgram(programInfo.Id, length, skip);
    }

    public List<PreLaunchTask> ListByProgram(ProgramInfo programInfo, int length = -1, int skip = 0)
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

    public List<PreLaunchTask> ListEnabledByProgram(int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return preLaunchTaskDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public List<PreLaunchTask> ListEnabledByProgram(string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<PreLaunchTask>() : ListByProgram(programInfo.Id, length, skip);
    }

    public List<PreLaunchTask> ListEnabledByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ListEnabledByProgram(programInfo.Id, enabled, length, skip);
    }

    public int ForEach(Action<PreLaunchTask> action, int length = -1, int skip = 0)
    {
        return preLaunchTaskDao.ForEach(action, length, skip);
    }

    public int ForEachByProgram(Action<PreLaunchTask> action, int programId, int length = -1, int skip = 0)
    {
        return preLaunchTaskDao.ForEachByForeignKey(action, programId, length, skip);
    }

    public int ForEachByProgram(Action<PreLaunchTask> action, string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachByProgram(action, programInfo, length, skip);
    }

    public int ForEachByProgram(Action<PreLaunchTask> action, ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return ForEachByProgram(action, programInfo.Id, length, skip);
    }

    public int ForEachEnabledByProgram(Action<PreLaunchTask> action, int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return preLaunchTaskDao.ForEachEnabledByForeignKey(action, programId, enabled, length, skip);
    }

    public int ForEachEnabledByProgram(Action<PreLaunchTask> action, string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachEnabledByProgram(action, programInfo, enabled, length, skip);
    }

    public int ForEachEnabledByProgram(Action<PreLaunchTask> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ForEachEnabledByProgram(action, programInfo.Id, enabled, length, skip);
    }

    private readonly PreLaunchTaskDao preLaunchTaskDao;
    private readonly ProgramInfoDao programInfoDao;
}
