using Microsoft.Data.Sqlite;

namespace PreLaunchTaskr.Core.Utils.SqlUtils;

public class SqliteWritingExecutor : AbstractWritingSqlExecutor<SqliteCommand, SqliteDataReader, SqliteWritingExecutor>
{
    public SqliteWritingExecutor() : base() { }

    public SqliteWritingExecutor(SqliteCommand command) : base(command) { }

    public override SqliteWritingExecutor AddParameterWithValue(string parameter, object? value)
    {
        Command.Parameters.AddWithValue(parameter, value);
        return this;
    }
}
