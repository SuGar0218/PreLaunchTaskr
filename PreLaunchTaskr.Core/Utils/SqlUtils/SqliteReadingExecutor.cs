using Microsoft.Data.Sqlite;

namespace PreLaunchTaskr.Core.Utils.SqlUtils;

public class SqliteReadingExecutor : AbstractReadingSqlExecutor<SqliteCommand, SqliteDataReader, SqliteReadingExecutor>
{
    public SqliteReadingExecutor() : base() { }

    public SqliteReadingExecutor(SqliteCommand command) : base(command) { }

    public override SqliteReadingExecutor AddParameterWithValue(string parameter, object? value)
    {
        Command.Parameters.AddWithValue(parameter, value);
        return this;
    }
}
