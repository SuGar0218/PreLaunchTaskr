namespace PreLaunchTaskr.Core.Entities;

public class ProgramInfo : AbstractEntity
{
    public string Path { get; set; }
    public bool Enabled { get; set; }

    public ProgramInfo(string path, bool enabled) : this(-1, path, enabled) { }

    public ProgramInfo(int id, string path, bool enabled) : base(id)
    {
        Path = path;
        Enabled = enabled;
    }
}
