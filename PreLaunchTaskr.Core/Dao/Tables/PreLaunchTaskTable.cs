using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Dao.Tables;

public class PreLaunchTaskTable : AbstractTable<PreLaunchTaskTable>
{
    public const string Name = "prelaunch_task";

    public class Field
    {
        public const string Id = "id";
        public const string ProgramId = "program_id";
        public const string TaskPath = "task_path";
        public const string AcceptProgramArgs = "accept_program_args";
        public const string IncludeAttachedArgs = "include_attached_args";
        public const string Enabled = "enabled";
    }

    public class Column
    {
        public static readonly string Id = GetColumnName(Name, Field.Id);
        public static readonly string ProgramId = GetColumnName(Name, Field.ProgramId);
        public static readonly string BatchFilePath = GetColumnName(Name, Field.TaskPath);
        public static readonly string AcceptProgramArgs = GetColumnName(Name, Field.AcceptProgramArgs);
        public static readonly string IncludeAttachedArgs = GetColumnName(Name, Field.IncludeAttachedArgs);
        public static readonly string Enabled = GetColumnName(Name, Field.Enabled);
    }

    public static string PrimaryKeyField => Field.Id;
    public static string PrimaryKeyColumn => Column.Id;

    public static string ForeignKeyField => Field.ProgramId;
    public static string ForeignKeyColumn => Column.ProgramId;

    public static string CreateIfNotExistsSQL => $@"
        CREATE TABLE IF NOT EXISTS {Name}
        (
            {Field.Id} INTEGER PRIMARY KEY AUTOINCREMENT,
            {Field.ProgramId} INTEGER NOT NULL,
            {Field.TaskPath} TEXT NOT NULL,
            {Field.AcceptProgramArgs} BOOL NOT NULL,
            {Field.IncludeAttachedArgs} BOOL NOT NULL,
            {Field.Enabled} BOOL NOT NULL,
            FOREIGN KEY ({Field.ProgramId}) REFERENCES {ProgramTable.Name}({ProgramTable.Field.Id})
        );
    ";

    /// <summary>
    /// [Id, ProgramId, TaskPath, AcceptProgramArgs, IncludeAttachedArgs, Enabled]
    /// </summary>
    protected override string[] _AllFields { get; } = new string[] {
        Field.Id,
        Field.ProgramId,
        Field.TaskPath,
        Field.AcceptProgramArgs,
        Field.IncludeAttachedArgs,
        Field.Enabled
    };

    /// <summary>
    /// [Id, ProgramId, TaskPath, AcceptProgramArgs, IncludeAttachedArgs, Enabled]
    /// </summary>
    protected override string[] _AllColumns { get; } = new string[] {
        Column.Id,
        Column.ProgramId,
        Column.BatchFilePath,
        Column.AcceptProgramArgs,
        Column.IncludeAttachedArgs,
        Column.Enabled
    };
}
