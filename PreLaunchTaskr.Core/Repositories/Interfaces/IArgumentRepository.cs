using PreLaunchTaskr.Core.Entities;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IArgumentRepository
{
    public List<AttachedArgument> ListAttachedArguments(int length = -1, int skip = 0);

    public List<AttachedArgument> ListAttachedArgumentsByProgram(int programId, int length = -1, int skip = 0);
    public List<AttachedArgument> ListAttachedArgumentsByProgram(string path, int length = -1, int skip = 0);
    public List<AttachedArgument> ListAttachedArgumentsByProgram(ProgramInfo programInfo, int length = -1, int skip = 0);

    public List<AttachedArgument> ListEnabledAttachedArgumentsByProgram(int programId, bool enabled = true, int length = -1, int skip = 0);
    public List<AttachedArgument> ListEnabledAttachedArgumentsByProgram(string path, bool enabled = true, int length = -1, int skip = 0);
    public List<AttachedArgument> ListEnabledAttachedArgumentsByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public int ForEachAttachedArguments(Action<AttachedArgument> action, int length = -1, int skip = 0);

    public int ForEachAttachedArgumentsByProgram(Action<AttachedArgument> action, int programId, int length = -1, int skip = 0);
    public int ForEachAttachedArgumentsByProgram(Action<AttachedArgument> action, string path, int length = -1, int skip = 0);
    public int ForEachAttachedArgumentsByProgram(Action<AttachedArgument> action, ProgramInfo programInfo, int length = -1, int skip = 0);

    public int ForEachEnabledAttachedArgumentsByProgram(Action<AttachedArgument> action, int programId, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledAttachedArgumentsByProgram(Action<AttachedArgument> action, string path, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledAttachedArgumentsByProgram(Action<AttachedArgument> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public AttachedArgument? GetAttachedArgumentById(int id);

    public AttachedArgument? AddAttachedArgument(AttachedArgument attachedArgument);
    public bool UpdateAttachedArgument(AttachedArgument attachedArgument);

    public bool RemoveAttachedArgumentById(int id);
    public int ClearAttachedArgumentsForProgram(int programId);
    public int ClearAttachedArgumentsForProgram(string path);
    public int ClearAttachedArgumentsForProgram(ProgramInfo programInfo);


    public List<BlockedArgument> ListBlockedArguments(int length = -1, int skip = 0);

    public List<BlockedArgument> ListBlockedArgumentsByProgram(int programId, int length = -1, int skip = 0);
    public List<BlockedArgument> ListBlockedArgumentsByProgram(string path, int length = -1, int skip = 0);
    public List<BlockedArgument> ListBlockedArgumentsByProgram(ProgramInfo programInfo, int length = -1, int skip = 0);

    public List<BlockedArgument> ListEnabledBlockedArgumentsByProgram(int programId, bool enabled = true, int length = -1, int skip = 0);
    public List<BlockedArgument> ListEnabledBlockedArgumentsByProgram(string path, bool enabled = true, int length = -1, int skip = 0);
    public List<BlockedArgument> ListEnabledBlockedArgumentsByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public int ForEachBlockedArguments(Action<BlockedArgument> action, int length = -1, int skip = 0);

    public int ForEachBlockedArgumentsByProgram(Action<BlockedArgument> action, int programId, int length = -1, int skip = 0);
    public int ForEachBlockedArgumentsByProgram(Action<BlockedArgument> action, string path, int length = -1, int skip = 0);
    public int ForEachBlockedArgumentsByProgram(Action<BlockedArgument> action, ProgramInfo programInfo, int length = -1, int skip = 0);

    public int ForEachEnabledBlockedArgumentsByProgram(Action<BlockedArgument> action, int programId, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledBlockedArgumentsByProgram(Action<BlockedArgument> action, string path, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledBlockedArgumentsByProgram(Action<BlockedArgument> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public BlockedArgument? GetBlockedArgumentById(int id);

    public BlockedArgument? AddBlockedArgument(BlockedArgument blockedArgument);
    public bool UpdateBlockedArgument(BlockedArgument blockedArgument);

    public bool RemoveBlockedArgumentById(int id);
    public int ClearBlockedArgumentsForProgram(int programId);
    public int ClearBlockedArgumentsForProgram(string path);
    public int ClearBlockedArgumentsForProgram(ProgramInfo programInfo);


    public int Clear();
}
