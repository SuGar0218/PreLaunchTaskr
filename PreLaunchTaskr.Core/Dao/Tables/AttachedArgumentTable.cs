namespace PreLaunchTaskr.Core.Dao.Tables;

/// <summary>
/// 保存每个程序要附加的参数
/// <br/>
/// 若一个程序要附加多个参数，表中会存有多个散落的ProgramId相同的不同参数。
/// </summary>
public class AttachedArgumentTable : AbstractTable<AttachedArgumentTable>
{
    public const string Name = "attached_argument";

    public class Field
    {
        public const string Id = "id";
        public const string ProgramId = "program_id";
        public const string Argument = "argument";
        public const string Enabled = "enabled";
    }

    public class Column
    {
        public static readonly string Id = GetColumnName(Name, Field.Id);
        public static readonly string ProgramId = GetColumnName(Name, Field.ProgramId);
        public static readonly string Argument = GetColumnName(Name, Field.Argument);
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
            {Field.Argument} TEXT NOT NULL,
            {Field.Enabled} BOOL NOT NULL,
            FOREIGN KEY({Field.ProgramId}) REFERENCES {ProgramTable.Name}({Field.Id})
        );
    ";

    /// <summary>
    /// [Id, ProgramId, Argument, Enabled]
    /// </summary>
    protected override string[] _AllFields { get; } = new string[] { Field.Id, Field.ProgramId, Field.Argument, Field.Enabled };

    /// <summary>
    /// [Id, ProgramId, Argument, Enabled]
    /// </summary>
    protected override string[] _AllColumns { get; } = new string[] { Column.Id, Column.ProgramId, Column.Argument, Column.Enabled };
}
