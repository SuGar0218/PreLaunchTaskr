using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PreLaunchTaskr.Core.Dao.Tables;

/// <summary>
/// 保存每个程序的专属环境变量
/// <br/>
/// 若一个程序的环境变量有多个，表中会存有多个散落的键值对。
/// </summary>
public class EnvironmentVariableTable : AbstractTable<EnvironmentVariableTable>
{
    public const string Name = "environment_variable";

    public class Field
    {
        public const string Id = "id";
        public const string ProgramId = "program_id";
        public const string Key = "key";
        public const string Value = "value";
        public const string Enabled = "enabled";
    }

    public class Column
    {
        public static readonly string Id = GetColumnName(Name, Field.Id);
        public static readonly string ProgramId = GetColumnName(Name, Field.ProgramId);
        public static readonly string Key = GetColumnName(Name, Field.Key);
        public static readonly string Value = GetColumnName(Name, Field.Value);
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
            {Field.Key} TEXT NOT NULL,
            {Field.Value} TEXT NOT NULL,
            {Field.Enabled} BOOL NOT NULL,
            FOREIGN KEY({Field.ProgramId}) REFERENCES {ProgramTable.Name}({Field.Id})
        );
    ";

    /// <summary>
    /// [Id, ProgramId, Key, Value, Enabled]
    /// </summary>
    protected override string[] _AllFields { get; } = new string[] { Field.Id, Field.ProgramId, Field.Key, Field.Value, Field.Enabled };

    /// <summary>
    /// [Id, ProgramId, Key, Value, Enabled]
    /// </summary>
    protected override string[] _AllColumns { get; } = new string[] { Column.Id, Column.ProgramId, Column.Key, Column.Value, Column.Enabled };
}
