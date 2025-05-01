using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao.Abstract;
using PreLaunchTaskr.Core.Dao.Tables;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Utils.SqlUtils;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao;

public class EnvironmentVariableDao :
    AbstractSqliteDao,
    IDao<EnvironmentVariable>,
    IDaoWithPrimaryKey<int, EnvironmentVariable>,
    IDaoWithForeignKey<int, EnvironmentVariable>
{
    public EnvironmentVariableDao(SqliteConnection connection) : base(connection)
    {
        CreateTableIfNotExists();
    }

    public int Add(EnvironmentVariable data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        int inserted = new SqliteCommandTextBuilder()
            .InsertInto(EnvironmentVariableTable.Name, EnvironmentVariableTable.AllFields[1..])
            .Values("(" +
                $"${EnvironmentVariableTable.Field.ProgramId}, " +
                $"${EnvironmentVariableTable.Field.Key}, " +
                $"${EnvironmentVariableTable.Field.Value}, " +
                $"${EnvironmentVariableTable.Field.Enabled})")
            .GetWritingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Key, data.Key)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Value, data.Value)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Enabled, data.Enabled)
            .ExecuteNonQuery();

        if (inserted == 0)
            return -1;

        return new SqliteCommandTextBuilder()
            .Select(SqliteCommandTextBuilder.LastInsertRowId())
            .From(EnvironmentVariableTable.Name)
            .GetReadingExcutor(command)
            .ExecuteScalarReading<int>();
    }

    public int Clear()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(EnvironmentVariableTable.Name)
            .GetWritingExcutor(command)
            .ExecuteNonQuery();
    }

    public int ClearByForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(EnvironmentVariableTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, key)
            .ExecuteNonQuery();
    }

    public bool ExistsForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(EnvironmentVariableTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, key)
            .ExecuteScalarReading<bool>();
    }

    public bool ExistsPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(EnvironmentVariableTable.Name)
            .Where($"{EnvironmentVariableTable.Column.Id} = ${EnvironmentVariableTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Id, key)
            .ExecuteScalarReading<bool>();
    }

    public EnvironmentVariable? GetByPrimaryKey(int key)
    {
        //if (!ExistsPrimaryKey(key))
        //    return null;

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{EnvironmentVariableTable.Column.Id} = ${EnvironmentVariableTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Id, key)
            .ExecuteDataReading<EnvironmentVariable>(ReadEnvironmentVariable);
    }

    public List<EnvironmentVariable> List(int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<EnvironmentVariable>(ReadEnvironmentVariable);
    }

    public List<EnvironmentVariable> ListByForeignKey(int key, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{EnvironmentVariableTable.Column.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, key)
            .ExecuteListReading<EnvironmentVariable>(ReadEnvironmentVariable);
    }

    public List<EnvironmentVariable> ListEnabledByForeignKey(int key, bool enabled, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{EnvironmentVariableTable.Column.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}")
            .And($"{EnvironmentVariableTable.Column.Enabled} = ${EnvironmentVariableTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, key)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Enabled, enabled)
            .ExecuteListReading<EnvironmentVariable>(ReadEnvironmentVariable);
    }

    public int RemoveByPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(EnvironmentVariableTable.Name)
            .Where($"{EnvironmentVariableTable.Column.Id} = ${EnvironmentVariableTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Id, key)
            .ExecuteNonQuery();
    }

    public int Update(EnvironmentVariable data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Update(EnvironmentVariableTable.Name)
            .Set(
                $"{EnvironmentVariableTable.Field.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}",
                $"{EnvironmentVariableTable.Field.Key} = ${EnvironmentVariableTable.Field.Key}",
                $"{EnvironmentVariableTable.Field.Value} = ${EnvironmentVariableTable.Field.Value}," +
                $"{EnvironmentVariableTable.Field.Enabled} = ${EnvironmentVariableTable.Field.Enabled}")
            .Where($"{EnvironmentVariableTable.Column.Id} = ${EnvironmentVariableTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Key, data.Key)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Value, data.Value)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Enabled, data.Enabled)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Id, data.Id)
            .ExecuteNonQuery();
    }

    private static EnvironmentVariable ReadEnvironmentVariable(SqliteDataReader reader)
    {
        return new EnvironmentVariable(
            reader.GetInt32(0),
            new ProgramInfo(reader.GetInt32(1), reader.GetString(6), reader.GetBoolean(7)),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetBoolean(4));
    }

    public void CreateTableIfNotExists()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = ProgramTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
        command.CommandText = EnvironmentVariableTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
    }

    public int ForEach(Action<EnvironmentVariable> action, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<EnvironmentVariable>(ReadEnvironmentVariable, action);
    }

    public int ForEachByForeignKey(Action<EnvironmentVariable> action, int key, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{EnvironmentVariableTable.Column.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, key)
            .ExecuteListReading<EnvironmentVariable>(ReadEnvironmentVariable, action);
    }

    public int ForEachEnabledByForeignKey(Action<EnvironmentVariable> action, int key, bool enabled, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(EnvironmentVariableTable.AllColumns, ProgramTable.AllColumns)
            .From(EnvironmentVariableTable.Name, ProgramTable.Name)
            .Where($"{EnvironmentVariableTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{EnvironmentVariableTable.Column.ProgramId} = ${EnvironmentVariableTable.Field.ProgramId}")
            .And($"{EnvironmentVariableTable.Column.Enabled} = ${EnvironmentVariableTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(EnvironmentVariableTable.Field.ProgramId, key)
            .AddParameterWithValue(EnvironmentVariableTable.Field.Enabled, enabled)
            .ExecuteListReading<EnvironmentVariable>(ReadEnvironmentVariable, action);
    }
}
