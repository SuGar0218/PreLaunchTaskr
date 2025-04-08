using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Entities;

public class EnvironmentVariable : AbstractEntity
{
    public EnvironmentVariable(ProgramInfo programInfo, string key, string value, bool enabled) : this(-1, programInfo, key, value, enabled) { }

    public EnvironmentVariable(int id, ProgramInfo programInfo, string key, string value, bool enabled) : base(-1)
    {
        ProgramInfo = programInfo;
        Key = key;
        Value = value;
        Enabled = enabled;
    }

    public ProgramInfo ProgramInfo { get; set; }
    public string Key { get; set; }
    public string Value{ get; set; }
    public bool Enabled { get; set; }
}
