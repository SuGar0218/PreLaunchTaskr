using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Implementations;

public class ProgramRepositoryImpl : IProgramRepository
{
    public ProgramRepositoryImpl(SqliteConnection connection) : this(
        new ProgramInfoDao(connection),
        new AttachedArgumentDao(connection),
        new BlockedArgumentDao(connection),
        new PreLaunchTaskDao(connection),
        new EnvironmentVariableDao(connection))
    { }

    public ProgramRepositoryImpl(
        ProgramInfoDao programInfoDao,
        AttachedArgumentDao attachedArgumentDao,
        BlockedArgumentDao blockedArgumentDao,
        PreLaunchTaskDao preLaunchTaskDao,
        EnvironmentVariableDao environmentVariableDao)
    {
        this.programInfoDao = programInfoDao;
        this.attachedArgumentDao = attachedArgumentDao;
        this.blockedArgumentDao = blockedArgumentDao;
        this.preLaunchTaskDao = preLaunchTaskDao;
        this.environmentVariableDao = environmentVariableDao;
    }

    public ProgramInfo? Add(ProgramInfo programInfo)
    {
        int id = programInfoDao.Add(programInfo);
        if (id == -1)
            return null;
        programInfo.Id = id;
        return programInfo;
    }

    public int Clear()
    {
        attachedArgumentDao.Clear();
        blockedArgumentDao.Clear();
        preLaunchTaskDao.Clear();
        environmentVariableDao.Clear();
        return programInfoDao.Clear();
    }

    public ProgramInfo? GetById(int id)
    {
        return programInfoDao.GetByPrimaryKey(id);
    }

    public ProgramInfo? GetByPath(string path)
    {
        return programInfoDao.GetByUniqueKey(path);
    }

    public IList<ProgramInfo> List(int length, int skip)
    {
        return programInfoDao.List(length, skip);
    }

    public IList<ProgramInfo> ListAll()
    {
        return programInfoDao.List();
    }

    public bool RemoveById(int id)
    {
        attachedArgumentDao.ClearByForeignKey(id);
        blockedArgumentDao.ClearByForeignKey(id);
        preLaunchTaskDao.ClearByForeignKey(id);
        environmentVariableDao.ClearByForeignKey(id);
        return programInfoDao.RemoveByPrimaryKey(id) > 0;
    }

    public bool RemoveByPath(string path)
    {
        ProgramInfo? programInfo = GetByPath(path);
        return programInfo is null ? false : RemoveById(programInfo.Id);
    }

    public bool Update(ProgramInfo programInfo)
    {
        return programInfoDao.Update(programInfo) > 0;
    }

    public IList<ProgramInfo> ListAllEnabled(bool enabled)
    {
        return programInfoDao.ListEnabled(enabled);
    }

    public IList<ProgramInfo> ListEnabled(bool enabled, int length, int skip = 0)
    {
        return programInfoDao.ListEnabled(enabled, length, skip);
    }

    private readonly ProgramInfoDao programInfoDao;
    private readonly AttachedArgumentDao attachedArgumentDao;
    private readonly BlockedArgumentDao blockedArgumentDao;
    private readonly PreLaunchTaskDao preLaunchTaskDao;
    private readonly EnvironmentVariableDao environmentVariableDao;
}
