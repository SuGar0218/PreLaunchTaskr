using PreLaunchTaskr.Core.Entities;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Repositories.Interfaces;

public interface IProgramRepository
{
    public IList<ProgramInfo> ListAll();
    public IList<ProgramInfo> List(int length, int skip = 0);

    public IList<ProgramInfo> ListAllEnabled(bool enabled);
    public IList<ProgramInfo> ListEnabled(bool enabled, int length, int skip = 0);

    public ProgramInfo? GetById(int id);
    public ProgramInfo? GetByPath(string path);

    public ProgramInfo? Add(ProgramInfo programInfo);
    public bool Update(ProgramInfo programInfo);

    public bool RemoveById(int id);
    public bool RemoveByPath(string path);
    public int Clear();
}
