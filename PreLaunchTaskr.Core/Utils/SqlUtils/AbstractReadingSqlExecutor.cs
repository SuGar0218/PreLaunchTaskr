using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace PreLaunchTaskr.Core.Utils.SqlUtils;

public abstract class AbstractReadingSqlExecutor<TDbCommand, TDbDataReader, TSelf> : AbstractSqlExecutor<TDbCommand, TDbDataReader, TSelf>
    where TDbCommand : DbCommand
    where TDbDataReader : DbDataReader
{
    public AbstractReadingSqlExecutor() : base() { }

    protected AbstractReadingSqlExecutor(TDbCommand command) : base(command) { }

    public List<TData> ExecuteListReading<TData>(Func<TDbDataReader, TData> read, CommandBehavior behavior = CommandBehavior.Default)
    {
        List<TData> data = new();
        using (TDbDataReader reader = (TDbDataReader) Command.ExecuteReader(behavior))
        {
            while (reader.Read())
            {
                data.Add(read.Invoke(reader));
            }
        }
        return data;
    }

    public int ExecuteListReading<TData>(Func<TDbDataReader, TData> read, Action<TData> action, CommandBehavior behavior = CommandBehavior.Default)
    {
        int count = 0;
        using (TDbDataReader reader = (TDbDataReader) Command.ExecuteReader(behavior))
        {
            while (reader.Read())
            {
                action.Invoke(read.Invoke(reader));
                count++;
            }
        }
        return count;
    }

    public List<TData> ExecuteColumnReading<TData>(int column = 0, CommandBehavior behavior = 0)
    {
        List<TData> data = new();
        using (TDbDataReader reader = (TDbDataReader) Command.ExecuteReader())
        {
            while (reader.Read())
            {
                data.Add(reader.GetFieldValue<TData>(column));
            }
        }
        return data;
    }

    public int ExecuteColumnReading<TColumn>(Action<TColumn> action, int column = 0, CommandBehavior behavior = 0)
    {
        int count = 0;
        using (TDbDataReader reader = (TDbDataReader) Command.ExecuteReader())
        {
            while (reader.Read())
            {
                action.Invoke(reader.GetFieldValue<TColumn>(column));
                count++;
            }
        }
        return count;
    }

    public object? ExecuteScalarReading() => Command.ExecuteScalar();

    public T? ExecuteScalarReading<T>()
    {
        using TDbDataReader reader = (TDbDataReader) Command.ExecuteReader();
        return reader.Read() ? reader.GetFieldValue<T>(0) : default;
    }

    public TData? ExecuteDataReading<TData>(Func<TDbDataReader, TData> read)
    {
        using TDbDataReader reader = (TDbDataReader) Command.ExecuteReader();
        return reader.Read() ? read.Invoke(reader) : default;
    }
}
