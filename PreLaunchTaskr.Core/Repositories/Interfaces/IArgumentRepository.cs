using PreLaunchTaskr.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IArgumentRepository
{
    public IList<AttachedArgument> ListAllAttachedArguments();
    public IList<AttachedArgument> ListAttachedArguments(int length, int skip = 0);

    public IList<AttachedArgument> ListAttachedArgumentsByProgram(int programId);
    public IList<AttachedArgument> ListAttachedArgumentsByProgram(string path);
    public IList<AttachedArgument> ListAttachedArgumentsByProgram(ProgramInfo programInfo);
    public IList<AttachedArgument> ListAttachedArgumentsByProgram(int programId, int length, int skip = 0);
    public IList<AttachedArgument> ListAttachedArgumentsByProgram(string path, int length, int skip = 0);
    public IList<AttachedArgument> ListAttachedArgumentsByProgram(ProgramInfo programInfo, int length, int skip = 0);

    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(int programId, bool enabled);
    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(string path, bool enabled);
    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(ProgramInfo programInfo, bool enabled);
    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(int programId, bool enabled, int length, int skip = 0);
    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(string path, bool enabled, int length, int skip = 0);
    public IList<AttachedArgument> ListEnabledAttachedArgumentsByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0);

    public AttachedArgument? GetAttachedArgumentById(int id);

    public AttachedArgument? AddAttachedArgument(AttachedArgument attachedArgument);
    public bool UpdateAttachedArgument(AttachedArgument attachedArgument);

    public bool RemoveAttachedArgumentById(int id);
    public int ClearAttachedArgumentsForProgram(int programId);
    public int ClearAttachedArgumentsForProgram(string path);
    public int ClearAttachedArgumentsForProgram(ProgramInfo programInfo);


    public IList<BlockedArgument> ListAllBlockedArguments();
    public IList<BlockedArgument> ListBlockedArguments(int length, int skip = 0);

    public IList<BlockedArgument> ListBlockedArgumentsByProgram(int programId);
    public IList<BlockedArgument> ListBlockedArgumentsByProgram(string path);
    public IList<BlockedArgument> ListBlockedArgumentsByProgram(ProgramInfo programInfo);
    public IList<BlockedArgument> ListBlockedArgumentsByProgram(int programId, int length, int skip = 0);
    public IList<BlockedArgument> ListBlockedArgumentsByProgram(string path, int length, int skip = 0);
    public IList<BlockedArgument> ListBlockedArgumentsByProgram(ProgramInfo programInfo, int length, int skip = 0);

    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(int programId, bool enabled);
    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(string path, bool enabled);
    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(ProgramInfo programInfo, bool enabled);
    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(int programId, bool enabled, int length, int skip = 0);
    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(string path, bool enabled, int length, int skip = 0);
    public IList<BlockedArgument> ListEnabledBlockedArgumentsByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0);

    public BlockedArgument? GetBlockedArgumentById(int id);

    public BlockedArgument? AddBlockedArgument(BlockedArgument blockedArgument);
    public bool UpdateBlockedArgument(BlockedArgument blockedArgument);

    public bool RemoveBlockedArgumentById(int id);
    public int ClearBlockedArgumentsForProgram(int programId);
    public int ClearBlockedArgumentsForProgram(string path);
    public int ClearBlockedArgumentsForProgram(ProgramInfo programInfo);


    public int Clear();
}
