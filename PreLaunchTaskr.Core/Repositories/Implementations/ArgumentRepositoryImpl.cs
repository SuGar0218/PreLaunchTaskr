using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Common.Utils;
using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Implementations;

public class ArgumentRepositoryImpl : IArgumentRepository
{
    public ArgumentRepositoryImpl(SqliteConnection connection) : this(
        new AttachedArgumentDao(connection),
        new BlockedArgumentDao(connection),
        new ProgramInfoDao(connection))
    {
    }

    public ArgumentRepositoryImpl(
        AttachedArgumentDao attachedArgumentDao,
        BlockedArgumentDao blockedArgumentDao,
        ProgramInfoDao programInfoDao)
    {
        this.attachedArgumentDao = attachedArgumentDao;
        this.blockedArgumentDao = blockedArgumentDao;
        this.programInfoDao = programInfoDao;
    }

    public AttachedArgument? AddAttachedArgument(AttachedArgument attachedArgument)
    {
        int id = attachedArgumentDao.Add(attachedArgument);
        if (id == -1)
            return null;
        attachedArgument.Id = id;
        return attachedArgument;
    }

    public BlockedArgument? AddBlockedArgument(BlockedArgument blockedArgument)
    {
        int id = blockedArgumentDao.Add(blockedArgument);
        if (id == -1)
            return null;
        blockedArgument.Id = id;
        return blockedArgument;
    }

    public int Clear()
    {
        return attachedArgumentDao.Clear() + blockedArgumentDao.Clear();
    }

    public int ClearAttachedArgumentsForProgram(int programId)
    {
        return attachedArgumentDao.ClearByForeignKey(programId);
    }

    public int ClearAttachedArgumentsForProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ClearAttachedArgumentsForProgram(programInfo);
    }

    public int ClearAttachedArgumentsForProgram(ProgramInfo programInfo)
    {
        return ClearAttachedArgumentsForProgram(programInfo.Id);
    }

    public int ClearBlockedArgumentsForProgram(int programId)
    {
        return blockedArgumentDao.ClearByForeignKey(programId);
    }

    public int ClearBlockedArgumentsForProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ClearBlockedArgumentsForProgram(programInfo);
    }

    public int ClearBlockedArgumentsForProgram(ProgramInfo programInfo)
    {
        return ClearBlockedArgumentsForProgram(programInfo.Id);
    }

    public AttachedArgument? GetAttachedArgumentById(int id)
    {
        return attachedArgumentDao.GetByPrimaryKey(id);
    }

    public BlockedArgument? GetBlockedArgumentById(int id)
    {
        return blockedArgumentDao.GetByPrimaryKey(id);
    }

    public List<AttachedArgument> ListAttachedArguments(int length = -1, int skip = 0)
    {
        return attachedArgumentDao.List(length, skip);
    }

    public List<AttachedArgument> ListAttachedArgumentsByProgram(int programId, int length = -1, int skip = 0)
    {
        return attachedArgumentDao.ListByForeignKey(programId, length, skip);
    }

    public List<AttachedArgument> ListAttachedArgumentsByProgram(string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<AttachedArgument>() : ListAttachedArgumentsByProgram(programInfo, length, skip);
    }

    public List<AttachedArgument> ListAttachedArgumentsByProgram(ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return ListAttachedArgumentsByProgram(programInfo.Id, length, skip);
    }

    public List<BlockedArgument> ListBlockedArguments(int length = -1, int skip = 0)
    {
        return blockedArgumentDao.List(length, skip);
    }

    public List<BlockedArgument> ListBlockedArgumentsByProgram(int programId, int length = -1, int skip = 0)
    {
        return blockedArgumentDao.ListByForeignKey(programId, length, skip);
    }

    public List<BlockedArgument> ListBlockedArgumentsByProgram(string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<BlockedArgument>() : ListBlockedArgumentsByProgram(programInfo, length, skip);
    }

    public List<BlockedArgument> ListBlockedArgumentsByProgram(ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return ListBlockedArgumentsByProgram(programInfo.Id, length, skip);
    }

    public bool RemoveAttachedArgumentById(int id)
    {
        return attachedArgumentDao.RemoveByPrimaryKey(id) > 0;
    }

    public bool RemoveBlockedArgumentById(int id)
    {
        return blockedArgumentDao.RemoveByPrimaryKey(id) > 0;
    }

    public bool UpdateAttachedArgument(AttachedArgument attachedArgument)
    {
        return attachedArgumentDao.Update(attachedArgument) > 0;
    }

    public bool UpdateBlockedArgument(BlockedArgument blockedArgument)
    {
        return blockedArgumentDao.Update(blockedArgument) > 0;
    }

    public List<AttachedArgument> ListEnabledAttachedArgumentsByProgram(int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return attachedArgumentDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public List<AttachedArgument> ListEnabledAttachedArgumentsByProgram(string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<AttachedArgument>() : ListEnabledAttachedArgumentsByProgram(programInfo, enabled, length, skip);
    }

    public List<AttachedArgument> ListEnabledAttachedArgumentsByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ListEnabledAttachedArgumentsByProgram(programInfo.Id, enabled, length, skip);
    }

    public List<BlockedArgument> ListEnabledBlockedArgumentsByProgram(int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return blockedArgumentDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public List<BlockedArgument> ListEnabledBlockedArgumentsByProgram(string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<BlockedArgument>() : ListEnabledBlockedArgumentsByProgram(programInfo, enabled, length, skip);
    }

    public List<BlockedArgument> ListEnabledBlockedArgumentsByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ListEnabledBlockedArgumentsByProgram(programInfo.Id, enabled, length, skip);
    }

    public int ForEachAttachedArguments(Action<AttachedArgument> action, int length = -1, int skip = 0)
    {
        return attachedArgumentDao.ForEach(action, length, skip);
    }

    public int ForEachAttachedArgumentsByProgram(Action<AttachedArgument> action, int programId, int length = -1, int skip = 0)
    {
        return attachedArgumentDao.ForEachByForeignKey(action, programId, length, skip);
    }

    public int ForEachAttachedArgumentsByProgram(Action<AttachedArgument> action, string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachAttachedArgumentsByProgram(action, programInfo, length, skip);
    }

    public int ForEachAttachedArgumentsByProgram(Action<AttachedArgument> action, ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return attachedArgumentDao.ForEachByForeignKey(action, programInfo.Id, length, skip);
    }

    public int ForEachEnabledAttachedArgumentsByProgram(Action<AttachedArgument> action, int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return attachedArgumentDao.ForEachEnabledByForeignKey(action, programId, enabled, length, skip);
    }

    public int ForEachEnabledAttachedArgumentsByProgram(Action<AttachedArgument> action, string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachEnabledAttachedArgumentsByProgram(action, programInfo, enabled, length, skip);
    }

    public int ForEachEnabledAttachedArgumentsByProgram(Action<AttachedArgument> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ForEachEnabledAttachedArgumentsByProgram(action, programInfo.Id, enabled, length, skip);
    }

    public int ForEachBlockedArguments(Action<BlockedArgument> action, int length = -1, int skip = 0)
    {
        return blockedArgumentDao.ForEach(action, length, skip);
    }

    public int ForEachBlockedArgumentsByProgram(Action<BlockedArgument> action, int programId, int length = -1, int skip = 0)
    {
        return blockedArgumentDao.ForEachByForeignKey(action, programId, length, skip);
    }

    public int ForEachBlockedArgumentsByProgram(Action<BlockedArgument> action, string path, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachBlockedArgumentsByProgram(action, programInfo, length, skip);
    }

    public int ForEachBlockedArgumentsByProgram(Action<BlockedArgument> action, ProgramInfo programInfo, int length = -1, int skip = 0)
    {
        return ForEachBlockedArgumentsByProgram(action, programInfo.Id, length, skip);
    }

    public int ForEachEnabledBlockedArgumentsByProgram(Action<BlockedArgument> action, int programId, bool enabled = true, int length = -1, int skip = 0)
    {
        return blockedArgumentDao.ForEachEnabledByForeignKey(action, programId, enabled, length, skip);
    }

    public int ForEachEnabledBlockedArgumentsByProgram(Action<BlockedArgument> action, string path, bool enabled = true, int length = -1, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? 0 : ForEachEnabledBlockedArgumentsByProgram(action, programInfo, enabled, length, skip);
    }

    public int ForEachEnabledBlockedArgumentsByProgram(Action<BlockedArgument> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0)
    {
        return ForEachEnabledBlockedArgumentsByProgram(action, programInfo.Id, enabled, length, skip);
    }

    private readonly AttachedArgumentDao attachedArgumentDao;
    private readonly BlockedArgumentDao blockedArgumentDao;
    private readonly ProgramInfoDao programInfoDao;
}
