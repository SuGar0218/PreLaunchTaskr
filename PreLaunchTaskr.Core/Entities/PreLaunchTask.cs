using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Entities;

public class PreLaunchTask : AbstractEntity
{
    public PreLaunchTask(
        ProgramInfo programInfo,
        string taskPath,
        bool acceptProgramArgs,
        bool includeAttachedArgs,
        bool enabled) : this(-1, programInfo, taskPath, acceptProgramArgs, includeAttachedArgs, enabled) { }

    public PreLaunchTask(
        int id,
        ProgramInfo programInfo,
        string taskPath,
        bool acceptProgramArgs,
        bool includeAttachedArgs,
        bool enabled) : base(id)
    {
        ProgramInfo = programInfo;
        TaskPath = taskPath;
        AcceptProgramArgs = acceptProgramArgs;
        IncludeAttachedArgs = includeAttachedArgs;
        Enabled = enabled;
    }

    public ProgramInfo ProgramInfo { get; set; }
    public string TaskPath { get; set; }
    public bool AcceptProgramArgs { get; set; }  // 是否接受正在启动的程序的参数
    public bool IncludeAttachedArgs { get; set; }  // 接受的参数是否包含用户附加的
    public bool Enabled { get; set; }
}
