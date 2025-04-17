using PreLaunchTaskr.Core.Entities;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IEnvironmentVariableRepository
{
    public IList<EnvironmentVariable> ListAll();
    public IList<EnvironmentVariable> List(int length, int skip = 0);

    public IList<EnvironmentVariable> ListByProgram(int programId);
    public IList<EnvironmentVariable> ListByProgram(string path);
    public IList<EnvironmentVariable> ListByProgram(ProgramInfo programInfo);
    public IList<EnvironmentVariable> ListByProgram(int programId, int length, int skip = 0);
    public IList<EnvironmentVariable> ListByProgram(string path, int length, int skip = 0);
    public IList<EnvironmentVariable> ListByProgram(ProgramInfo programInfo, int length, int skip = 0);

    public IList<EnvironmentVariable> ListEnabledByProgram(int programId, bool enabled);
    public IList<EnvironmentVariable> ListEnabledByProgram(string path, bool enabled);
    public IList<EnvironmentVariable> ListEnabledByProgram(ProgramInfo programInfo, bool enabled);
    public IList<EnvironmentVariable> ListEnabledByProgram(int programId, bool enabled, int length, int skip = 0);
    public IList<EnvironmentVariable> ListEnabledByProgram(string path, bool enabled, int length, int skip = 0);
    public IList<EnvironmentVariable> ListEnabledByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0);

    public EnvironmentVariable? GetById(int id);
    public EnvironmentVariable? GetByPath(string path);

    public EnvironmentVariable? Add(EnvironmentVariable variable);
    public bool Update(EnvironmentVariable variable);

    public bool RemoveById(int id);
    public int ClearForProgram(int programId);
    public int ClearForProgram(string path);
    public int ClearForProgram(ProgramInfo programInfo);
}
