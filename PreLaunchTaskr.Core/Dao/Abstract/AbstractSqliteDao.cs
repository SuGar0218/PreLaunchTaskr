using Microsoft.Data.Sqlite;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public abstract class AbstractSqliteDao
{
    public AbstractSqliteDao(SqliteConnection connection)
    {
        this.connection = connection;
    }

    /// <summary>
    /// 请使用：using SqliteConnection connection = OpenConnection()
    /// </summary>
    /// <returns></returns>
    protected SqliteConnection OpenConnection()
    {
        connection.Open();
        return connection;
    }

    private readonly SqliteConnection connection;
}
