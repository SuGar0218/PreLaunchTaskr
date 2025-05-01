using PreLaunchTaskr.Core.Entities;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IEnvironmentVariableRepository
{
    public List<EnvironmentVariable> List(int length = -1, int skip = 0);

    public List<EnvironmentVariable> ListByProgram(int programId, int length = -1, int skip = 0);
    public List<EnvironmentVariable> ListByProgram(string path, int length = -1, int skip = 0);
    public List<EnvironmentVariable> ListByProgram(ProgramInfo programInfo, int length = -1, int skip = 0);

    public List<EnvironmentVariable> ListEnabledByProgram(int programId, bool enabled = true, int length = -1, int skip = 0);
    public List<EnvironmentVariable> ListEnabledByProgram(string path, bool enabled = true, int length = -1, int skip = 0);
    public List<EnvironmentVariable> ListEnabledByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public int ForEach(Action<EnvironmentVariable> action, int length = -1, int skip = 0);

    public int ForEachByProgram(Action<EnvironmentVariable> action, int programId, int length = -1, int skip = 0);
    public int ForEachByProgram(Action<EnvironmentVariable> action, string path, int length = -1, int skip = 0);
    public int ForEachByProgram(Action<EnvironmentVariable> action, ProgramInfo programInfo, int length = -1, int skip = 0);

    public int ForEachEnabledByProgram(Action<EnvironmentVariable> action, int programId, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledByProgram(Action<EnvironmentVariable> action, string path, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledByProgram(Action<EnvironmentVariable> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public EnvironmentVariable? GetById(int id);
    public EnvironmentVariable? GetByPath(string path);

    public EnvironmentVariable? Add(EnvironmentVariable variable);
    public bool Update(EnvironmentVariable variable);

    public bool RemoveById(int id);
    public int ClearForProgram(int programId);
    public int ClearForProgram(string path);
    public int ClearForProgram(ProgramInfo programInfo);
}
