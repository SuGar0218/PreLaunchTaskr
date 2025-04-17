namespace PreLaunchTaskr.Core.Entities;

public class BlockedArgument : AbstractEntity
{
    public ProgramInfo ProgramInfo { get; set; }
    public string Argument { get; set; }
    public bool Enabled { get; set; }
    public bool IsRegex { get; set; }

    public BlockedArgument(ProgramInfo programInfo, string argument, bool enabled, bool isRegex)
        : this(-1, programInfo, argument, enabled, isRegex) { }

    public BlockedArgument(int id, ProgramInfo programInfo, string argument, bool enabled, bool isRegex)
        : base(id)
    {
        ProgramInfo = programInfo;
        Argument = argument;
        Enabled = enabled;
        IsRegex = isRegex;
    }
}
