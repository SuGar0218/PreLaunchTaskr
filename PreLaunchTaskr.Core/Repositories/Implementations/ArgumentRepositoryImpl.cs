using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Common.Utils;
using PreLaunchTaskr.Core.Dao;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Repositories.Interfaces;

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
        return programInfo is null ? 0 : ClearBlockedArgumentsForProgram(programInfo.Id);
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

    public IList<AttachedArgument> ListAllAttachedArguments()
    {
        return attachedArgumentDao.List();
    }

    public IList<BlockedArgument> ListAllBlockedArguments()
    {
        return blockedArgumentDao.List();
    }

    public IList<AttachedArgument> ListAttachedArguments(int length, int skip = 0)
    {
        return attachedArgumentDao.List(length, skip);
    }

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(int programId)
    {
        return attachedArgumentDao.ListByForeignKey(programId);
    }

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<AttachedArgument>() : ListAttachedArgumentsByProgram(programInfo.Id);
    }

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(ProgramInfo programInfo)
    {
        return ListAttachedArgumentsByProgram(programInfo.Id);
    }

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(int programId, int length, int skip = 0)
    {
        return attachedArgumentDao.ListByForeignKey(programId, length, skip);
    }

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(string path, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<AttachedArgument>() : ListAttachedArgumentsByProgram(programInfo.Id, length, skip);
    }

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(ProgramInfo programInfo, int length, int skip = 0)
    {
        return ListAttachedArgumentsByProgram(programInfo.Id, length, skip);
    }

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(int programId)
    {
        return blockedArgumentDao.ListByForeignKey(programId);
    }

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(string path)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<BlockedArgument>() : ListBlockedArgumentsByProgram(programInfo.Id);
    }

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(ProgramInfo programInfo)
    {
        return ListBlockedArgumentsByProgram(programInfo.Id);
    }

    public IList<BlockedArgument> ListBlockedArguments(int length, int skip = 0)
    {
        return blockedArgumentDao.List(length, skip);
    }

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(int programId, int length, int skip = 0)
    {
        return blockedArgumentDao.ListByForeignKey(programId, length, skip);
    }

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(string path, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<BlockedArgument>() : ListBlockedArgumentsByProgram(programInfo.Id, length, skip);
    }

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(ProgramInfo programInfo, int length, int skip = 0)
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

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(int programId, bool enabled)
    {
        return attachedArgumentDao.ListEnabledByForeignKey(programId, enabled);
    }

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(string path, bool enabled)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<AttachedArgument>() : ListEnabledAttachedArgumentsByProgram(programInfo.Id, enabled);
    }

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(ProgramInfo programInfo, bool enabled)
    {
        return ListEnabledAttachedArgumentsByProgram(programInfo.Id, enabled);
    }

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(int programId, bool enabled, int length, int skip = 0)
    {
        return attachedArgumentDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(string path, bool enabled, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<AttachedArgument>() : ListEnabledAttachedArgumentsByProgram(programInfo.Id, enabled, length, skip);
    }

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0)
    {
        return ListEnabledAttachedArgumentsByProgram(programInfo.Id, enabled, length, skip);
    }

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(int programId, bool enabled)
    {
        return blockedArgumentDao.ListEnabledByForeignKey(programId, enabled);
    }

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(string path, bool enabled)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<BlockedArgument>() : ListEnabledBlockedArgumentsByProgram(programInfo.Id, enabled);
    }

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(ProgramInfo programInfo, bool enabled)
    {
        return ListEnabledBlockedArgumentsByProgram(programInfo.Id, enabled);
    }

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(int programId, bool enabled, int length, int skip = 0)
    {
        return blockedArgumentDao.ListEnabledByForeignKey(programId, enabled, length, skip);
    }

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(string path, bool enabled, int length, int skip = 0)
    {
        ProgramInfo? programInfo = programInfoDao.GetByUniqueKey(path);
        return programInfo is null ? EmptyList.Of<BlockedArgument>() : ListEnabledBlockedArgumentsByProgram(programInfo.Id, enabled, length, skip);
    }

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0)
    {
        return ListEnabledBlockedArgumentsByProgram(programInfo.Id, enabled, length, skip);
    }

    private readonly AttachedArgumentDao attachedArgumentDao;
    private readonly BlockedArgumentDao blockedArgumentDao;
    private readonly ProgramInfoDao programInfoDao;
}
