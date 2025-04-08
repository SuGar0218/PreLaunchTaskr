using System.Data.Common;

namespace PreLaunchTaskr.Core.Utils.SqlUtils;

public abstract class AbstractSqlExecutor<TDbCommand, TDbDataReader, TSelf>
    where TDbCommand : DbCommand
    where TDbDataReader : DbDataReader
{
    public AbstractSqlExecutor() : this(null!) { }

    public AbstractSqlExecutor(TDbCommand command)
    {
        Command = command;
    }

    internal TDbCommand Command { get; set; }

    internal string CommandText
    {
        get => Command.CommandText;
        set
        {
            Command.CommandText = value;
            Command.Parameters.Clear();
        }
    }

    public abstract TSelf AddParameterWithValue(string parameter, object? value);
}
