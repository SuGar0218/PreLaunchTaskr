using PreLaunchTaskr.Core.Entities;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IPreLaunchTaskRepository
{
    public IList<PreLaunchTask> ListAll();
    public IList<PreLaunchTask> List(int length, int skip = 0);

    public IList<PreLaunchTask> ListByProgram(int programId);
    public IList<PreLaunchTask> ListByProgram(string path);
    public IList<PreLaunchTask> ListByProgram(ProgramInfo programInfo);
    public IList<PreLaunchTask> ListByProgram(int programId, int length, int skip = 0);
    public IList<PreLaunchTask> ListByProgram(string path, int length, int skip = 0);
    public IList<PreLaunchTask> ListByProgram(ProgramInfo programInfo, int length, int skip = 0);

    public IList<PreLaunchTask> ListEnabledByProgram(int programId, bool enabled);
    public IList<PreLaunchTask> ListEnabledByProgram(string path, bool enabled);
    public IList<PreLaunchTask> ListEnabledByProgram(ProgramInfo programInfo, bool enabled);
    public IList<PreLaunchTask> ListEnabledByProgram(int programId, bool enabled, int length, int skip = 0);
    public IList<PreLaunchTask> ListEnabledByProgram(string path, bool enabled, int length, int skip = 0);
    public IList<PreLaunchTask> ListEnabledByProgram(ProgramInfo programInfo, bool enabled, int length, int skip = 0);

    public PreLaunchTask? GetById(int id);

    public PreLaunchTask? Add(PreLaunchTask preLaunchTask);
    public bool Update(PreLaunchTask preLaunchTask);

    public bool RemoveById(int id);
    public int ClearForProgram(int programId);
    public int ClearForProgram(string path);
    public int ClearForProgram(ProgramInfo programInfo);
}
