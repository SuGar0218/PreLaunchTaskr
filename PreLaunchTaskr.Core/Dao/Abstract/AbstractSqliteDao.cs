using Microsoft.Data.Sqlite;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public abstract class AbstractSqliteDao
{
    public AbstractSqliteDao(SqliteConnection connection)
    {
        this.connection = connection;
    }

    protected SqliteConnection OpenConnection()
    {
        connection.Open();
        return connection;
    }

    private readonly SqliteConnection connection;
}
