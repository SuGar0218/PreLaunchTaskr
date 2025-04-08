using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
