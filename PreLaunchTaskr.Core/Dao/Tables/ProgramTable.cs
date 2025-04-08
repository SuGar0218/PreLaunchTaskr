namespace PreLaunchTaskr.Core.Dao.Tables;

public class ProgramTable : AbstractTable<ProgramTable>
{
    public const string Name = "program_table";

    public class Field
    {
        public const string Id = "id";
        public const string Path = "path";
        public const string Enabled = "enabled";
    }

    public class Column
    {
        public static string Id => GetColumnName(Name, Field.Id);
        public static string Path => GetColumnName(Name, Field.Path);
        public static string Enabled => GetColumnName(Name, Field.Enabled);
    }

    public static string PrimaryKeyField => Field.Id;
    public static string PrimaryKeyColumn => Column.Id;

    public static string UniqueKeyField => Field.Path;
    public static string UniqueKeyColumn => Column.Path;

    public static string CreateIfNotExistsSQL => $@"
        CREATE TABLE IF NOT EXISTS {Name}
        (
            {Field.Id} INTEGER PRIMARY KEY AUTOINCREMENT,
            {Field.Path} TEXT UNIQUE NOT NULL UNIQUE,
            {Field.Enabled} BOOL NOT NULL
        );
    ";

    /// <summary>
    /// [Id, Path, Enabled]
    /// </summary>
    protected override string[] _AllFields { get; } = new string[] { Field.Id, Field.Path, Field.Enabled };

    /// <summary>
    /// [Id, Path, Enabled]
    /// </summary>
    protected override string[] _AllColumns { get; } = new string[] { Column.Id, Column.Path, Column.Enabled };
}
