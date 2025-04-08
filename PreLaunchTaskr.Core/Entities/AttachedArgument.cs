namespace PreLaunchTaskr.Core.Entities;

public class AttachedArgument : AbstractEntity
{
    public ProgramInfo ProgramInfo { get; set; }
    public string Argument { get; set; }
    public bool Enabled { get; set; }

    public AttachedArgument(ProgramInfo programInfo, string argument, bool enabled) : this(-1, programInfo, argument, enabled) { }

    public AttachedArgument(int id, ProgramInfo programInfo, string argument, bool enabled) : base(id)
    {
        ProgramInfo = programInfo;
        Argument = argument;
        Enabled = enabled;
    }
}
