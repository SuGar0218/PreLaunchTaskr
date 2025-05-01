using PreLaunchTaskr.Core.Entities;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IProgramRepository
{
    public List<ProgramInfo> List(int length = -1, int skip = 0);
    public List<ProgramInfo> ListEnabled(bool enabled = true, int length = -1, int skip = 0);

    public int ForEach(Action<ProgramInfo> action, int length = -1, int skip = 0);
    public int ForEachEnabled(Action<ProgramInfo> action, bool enabled = true, int length = -1, int skip = 0);

    public ProgramInfo? GetById(int id);
    public ProgramInfo? GetByPath(string path);

    public ProgramInfo? Add(ProgramInfo programInfo);
    public bool Update(ProgramInfo programInfo);

    public bool RemoveById(int id);
    public bool RemoveByPath(string path);
    public int Clear();
}
