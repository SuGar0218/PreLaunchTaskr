using PreLaunchTaskr.Core.Entities;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IPreLaunchTaskRepository
{
    public List<PreLaunchTask> List(int length = -1, int skip = 0);

    public List<PreLaunchTask> ListByProgram(int programId, int length = -1, int skip = 0);
    public List<PreLaunchTask> ListByProgram(string path, int length = -1, int skip = 0);
    public List<PreLaunchTask> ListByProgram(ProgramInfo programInfo, int length = -1, int skip = 0);

    public List<PreLaunchTask> ListEnabledByProgram(int programId, bool enabled = true, int length = -1, int skip = 0);
    public List<PreLaunchTask> ListEnabledByProgram(string path, bool enabled = true, int length = -1, int skip = 0);
    public List<PreLaunchTask> ListEnabledByProgram(ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public int ForEach(Action<PreLaunchTask> action, int length = -1, int skip = 0);

    public int ForEachByProgram(Action<PreLaunchTask> action, int programId, int length = -1, int skip = 0);
    public int ForEachByProgram(Action<PreLaunchTask> action, string path, int length = -1, int skip = 0);
    public int ForEachByProgram(Action<PreLaunchTask> action, ProgramInfo programInfo, int length = -1, int skip = 0);

    public int ForEachEnabledByProgram(Action<PreLaunchTask> action, int programId, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledByProgram(Action<PreLaunchTask> action, string path, bool enabled = true, int length = -1, int skip = 0);
    public int ForEachEnabledByProgram(Action<PreLaunchTask> action, ProgramInfo programInfo, bool enabled = true, int length = -1, int skip = 0);

    public PreLaunchTask? GetById(int id);

    public PreLaunchTask? Add(PreLaunchTask preLaunchTask);
    public bool Update(PreLaunchTask preLaunchTask);

    public bool RemoveById(int id);
    public int ClearForProgram(int programId);
    public int ClearForProgram(string path);
    public int ClearForProgram(ProgramInfo programInfo);
}
