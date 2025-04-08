using System.Data.Common;

namespace PreLaunchTaskr.Core.Utils.SqlUtils;

public abstract class AbstractWritingSqlExecutor<TDbCommand, TDbDataReader, TSelf> : AbstractSqlExecutor<TDbCommand, TDbDataReader, TSelf>
    where TDbCommand : DbCommand
    where TDbDataReader : DbDataReader
{
    public AbstractWritingSqlExecutor() : base() { }

    protected AbstractWritingSqlExecutor(TDbCommand command) : base(command) { }

    public int ExecuteNonQuery()
    {
        return Command.ExecuteNonQuery();
    }
}
