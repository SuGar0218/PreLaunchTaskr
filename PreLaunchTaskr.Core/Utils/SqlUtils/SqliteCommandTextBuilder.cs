using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Extensions;

using System.Text;

namespace PreLaunchTaskr.Core.Utils.SqlUtils;

public class SqliteCommandTextBuilder
{
    public SqliteCommandTextBuilder(string? sql = null)
    {
        stringBuilder = new StringBuilder(sql);
    }

    public SqliteCommandTextBuilder Select1 => Select("1");

    public SqliteCommandTextBuilder Select(string columns)
    {
        stringBuilder.Append($"SELECT {columns}");
        return this;
    }

    public SqliteCommandTextBuilder Select(params string[] columns)
    {
        stringBuilder.Append($"SELECT {columns.ToString(',')}");
        return this;
    }

    public SqliteCommandTextBuilder Select(params string[][] columns)
    {
        StringBuilder columnsStringBuilder = new StringBuilder().AppendJoin(',', columns[0]);
        for (int i = 1; i < columns.Length; i++)
        {
            columnsStringBuilder.Append(',').AppendJoin(',', columns[i]);
        }
        return Select(columnsStringBuilder.ToString());
    }

    public SqliteCommandTextBuilder InsertInto(string table)
    {
        stringBuilder.Append($"INSERT INTO {table}");
        return this;
    }

    public SqliteCommandTextBuilder InsertInto(string table, string fields)
    {
        stringBuilder.Append($"INSERT INTO {table} ({fields})");
        return this;
    }

    public SqliteCommandTextBuilder InsertInto(string table, params string[] fields)
    {
        stringBuilder.Append($"INSERT INTO {table} ({fields.ToString(',')})");
        return this;
    }

    public SqliteCommandTextBuilder InsertOrReplaceInto(string table, string fields)
    {
        stringBuilder.Append($"INSERT OR REPLACE INTO {table} ({fields})");
        return this;
    }

    public SqliteCommandTextBuilder InsertOrReplaceInto(string table, params string[] fields)
    {
        stringBuilder.Append($"INSERT OR REPLACE INTO {table} ({fields.ToString(',')})");
        return this;
    }

    public SqliteCommandTextBuilder Update(string table)
    {
        stringBuilder.Append($"UPDATE {table}");
        return this;
    }

    public SqliteCommandTextBuilder DeleteFrom(string table)
    {
        stringBuilder.Append($"DELETE FROM {table}");
        return this;
    }

    public SqliteCommandTextBuilder Not(string condition)
    {
        stringBuilder.Append(' ').Append($"NOT {condition}");
        return this;
    }

    public SqliteCommandTextBuilder Exists(string sql)
    {
        stringBuilder.Append(' ').Append($"EXISTS ({sql})");
        return this;
    }
    public SqliteCommandTextBuilder NotExists(string sql) => Not(Exists(sql));

    public SqliteCommandTextBuilder From(string tables)
    {
        stringBuilder.Append(' ').Append($"FROM {tables}");
        return this;
    }

    public SqliteCommandTextBuilder From(params string[] tables)
    {
        stringBuilder.Append(' ').Append($"FROM {tables.ToString(',')}");
        return this;
    }

    public SqliteCommandTextBuilder Where(string condition)
    {
        stringBuilder.Append(' ').Append($"WHERE {condition}");
        return this;
    }

    public SqliteCommandTextBuilder Where(bool condition)
    {
        stringBuilder.Append(' ').Append($"WHERE {condition}");
        return this;
    }

    public SqliteCommandTextBuilder Join(string table)
    {
        stringBuilder.Append(' ').Append($"JOIN {table}");
        return this;
    }

    public SqliteCommandTextBuilder On(string condition)
    {
        stringBuilder.Append(' ').Append($"ON {condition}");
        return this;
    }

    public SqliteCommandTextBuilder And(string condition)
    {
        stringBuilder.Append(' ').Append($"AND {condition}");
        return this;
    }

    public SqliteCommandTextBuilder Or(string condition)
    {
        stringBuilder.Append(' ').Append($"OR {condition}");
        return this;
    }

    public SqliteCommandTextBuilder Limit(int limit)
    {
        stringBuilder.Append(' ').Append($"LIMIT {limit}");
        return this;
    }

    public SqliteCommandTextBuilder Limit(int limit, int offset)
    {
        stringBuilder.Append(' ').Append($"LIMIT {limit} OFFSET {offset}");
        return this;
    }

    public SqliteCommandTextBuilder OrderBy(OrderBy order, string columns)
    {
        stringBuilder.Append(' ').Append($"ORDER BY {columns} {order}");
        return this;
    }

    public SqliteCommandTextBuilder OrderBy(OrderBy order, params string[] columns)
    {
        stringBuilder.Append(' ').Append($"ORDER BY {columns.ToString(',')} {order}");
        return this;
    }

    public SqliteCommandTextBuilder GroupBy(string columns)
    {
        stringBuilder.Append(' ').Append($"GROUP BY {columns}");
        return this;
    }

    public SqliteCommandTextBuilder GroupBy(params string[] columns)
    {
        stringBuilder.Append(' ').Append($"GROUP BY {columns.ToString(',')}");
        return this;
    }

    public SqliteCommandTextBuilder Values(string values)
    {
        stringBuilder.Append(' ').Append($"VALUES {values}");
        return this;
    }

    public SqliteCommandTextBuilder Values(params string[] values)
    {
        stringBuilder.Append(' ').Append($"VALUES {values.ToString(',')}");
        return this;
    }

    public SqliteCommandTextBuilder Set(string changes)
    {
        stringBuilder.Append(' ').Append($"SET {changes}");
        return this;
    }

    public SqliteCommandTextBuilder Set(params string[] changes)
    {
        stringBuilder.Append(' ').Append($"SET {changes.ToString(',')}");
        return this;
    }

    public SqliteCommandTextBuilder InsertOrIgnoreInto(string table, string fields) => new($"INSERT OR IGNORE INTO {table} ({fields})");
    public SqliteCommandTextBuilder InsertOrIgnoreInto(string table, params string[] fields) => new($"INSERT OR IGNORE INTO {table} ({fields.ToString(',')})");

    public static string LastInsertRowId() => "last_insert_rowid()";

    public SqliteReadingExecutor GetReadingExcutor(SqliteCommand command)
    {
        SqliteReadingExecutor executor = new SqliteReadingExecutor();  // 不要简化对象初始化，因为设置 CommandText 时 Command 不能为 null
        executor.Command = command;
        executor.CommandText = stringBuilder.ToString();
        return executor;
    }

    public SqliteWritingExecutor GetWritingExcutor(SqliteCommand command)
    {
        SqliteWritingExecutor executor = new SqliteWritingExecutor();  // 不要简化对象初始化，因为设置 CommandText 时 Command 不能为 null
        executor.Command = command;
        executor.CommandText = stringBuilder.ToString();
        return executor;
    }

    public override string ToString() => stringBuilder.ToString();

    public static implicit operator string(SqliteCommandTextBuilder self) => self.ToString();

    internal SqliteCommandTextBuilder(StringBuilder stringBuilder)
    {
        this.stringBuilder = stringBuilder;
    }

    private readonly StringBuilder stringBuilder;
}
