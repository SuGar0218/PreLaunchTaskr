using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao.Abstract;
using PreLaunchTaskr.Core.Dao.Tables;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Utils.SqlUtils;

using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao;

public class BlockedArgumentDao :
    AbstractSqliteDao,
    IDao<BlockedArgument>,
    IDaoWithPrimaryKey<int, BlockedArgument>,
    IDaoWithForeignKey<int, BlockedArgument>
{
    public BlockedArgumentDao(SqliteConnection connection) : base(connection)
    {
        CreateTableIfNotExists();
    }

    public int Add(BlockedArgument data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        int inserted = new SqliteCommandTextBuilder()
            .InsertInto(BlockedArgumentTable.Name, BlockedArgumentTable.AllFields[1..])
            .Values($"(${BlockedArgumentTable.Field.ProgramId}, ${BlockedArgumentTable.Field.Argument}, ${BlockedArgumentTable.Field.Enabled}, ${BlockedArgumentTable.Field.IsRegex})")
            .GetWritingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(BlockedArgumentTable.Field.Argument, data.Argument)
            .AddParameterWithValue(BlockedArgumentTable.Field.Enabled, data.Enabled)
            .AddParameterWithValue(BlockedArgumentTable.Field.IsRegex, data.IsRegex)
            .ExecuteNonQuery();

        if (inserted == 0)
            return -1;

        return new SqliteCommandTextBuilder()
            .Select(SqliteCommandTextBuilder.LastInsertRowId())
            .From(BlockedArgumentTable.Name)
            .GetReadingExcutor(command)
            .ExecuteScalarReading<int>();
    }

    public int Clear()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(BlockedArgumentTable.Name)
            .GetWritingExcutor(command)
            .ExecuteNonQuery();
    }

    public int ClearByForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(BlockedArgumentTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, key)
            .ExecuteNonQuery();
    }

    public bool ExistsForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(BlockedArgumentTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Column.ProgramId, key)
            .ExecuteScalarReading<bool>();
    }

    public bool ExistsPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(BlockedArgumentTable.Name)
            .Where($"{BlockedArgumentTable.Column.Id} = ${BlockedArgumentTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.Id, key)
            .ExecuteScalarReading<bool>();
    }

    public BlockedArgument? GetByPrimaryKey(int key)
    {
        //if (!ExistsPrimaryKey(key))
        //    return null;

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.Id} = ${BlockedArgumentTable.Field.Id}")
            .And($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.Id, key)
            .ExecuteDataReading<BlockedArgument>(ReadBlockedArgument)!;
    }

    public List<BlockedArgument> List(int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<BlockedArgument>(ReadBlockedArgument);
    }

    public List<BlockedArgument> ListByForeignKey(int key, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{BlockedArgumentTable.Column.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, key)
            .ExecuteListReading<BlockedArgument>(ReadBlockedArgument);
    }

    public List<BlockedArgument> ListEnabledByForeignKey(int key, bool enabled = true, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{BlockedArgumentTable.Column.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}")
            .And($"{BlockedArgumentTable.Column.Enabled} = ${BlockedArgumentTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, key)
            .AddParameterWithValue(BlockedArgumentTable.Field.Enabled, enabled)
            .ExecuteListReading<BlockedArgument>(ReadBlockedArgument);
    }

    public int RemoveByPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(BlockedArgumentTable.Name)
            .Where($"{BlockedArgumentTable.Column.Id} = ${BlockedArgumentTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.Id, key)
            .ExecuteNonQuery();
    }

    public int Update(BlockedArgument data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Update(BlockedArgumentTable.Name)
            .Set(
                $"{BlockedArgumentTable.Field.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}",
                $"{BlockedArgumentTable.Field.Argument} = ${BlockedArgumentTable.Field.Argument}",
                $"{BlockedArgumentTable.Field.Enabled} = ${BlockedArgumentTable.Field.Enabled}",
                $"{BlockedArgumentTable.Field.IsRegex} = ${BlockedArgumentTable.Field.IsRegex}")
            .Where($"{BlockedArgumentTable.Column.Id} = ${BlockedArgumentTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(BlockedArgumentTable.Field.Argument, data.Argument)
            .AddParameterWithValue(BlockedArgumentTable.Field.Enabled, data.Enabled)
            .AddParameterWithValue(BlockedArgumentTable.Field.IsRegex, data.IsRegex)
            .AddParameterWithValue(BlockedArgumentTable.Field.Id, data.Id)
            .ExecuteNonQuery();
    }

    private static BlockedArgument ReadBlockedArgument(SqliteDataReader reader)
    {
        return new BlockedArgument(
            reader.GetInt32(0),
            new ProgramInfo(reader.GetInt32(1), reader.GetString(6), reader.GetBoolean(7)),
            reader.GetString(2),
            reader.GetBoolean(3),
            reader.GetBoolean(4));
    }

    public void CreateTableIfNotExists()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = ProgramTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
        command.CommandText = BlockedArgumentTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
    }

    public int ForEach(Action<BlockedArgument> action, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<BlockedArgument>(ReadBlockedArgument, action);
    }

    public int ForEachByForeignKey(Action<BlockedArgument> action, int key, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{BlockedArgumentTable.Column.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, key)
            .ExecuteListReading<BlockedArgument>(ReadBlockedArgument, action);
    }

    public int ForEachEnabledByForeignKey(Action<BlockedArgument> action, int key, bool enabled = true, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(BlockedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(BlockedArgumentTable.Name, ProgramTable.Name)
            .Where($"{BlockedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{BlockedArgumentTable.Column.ProgramId} = ${BlockedArgumentTable.Field.ProgramId}")
            .And($"{BlockedArgumentTable.Column.Enabled} = ${BlockedArgumentTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(BlockedArgumentTable.Field.ProgramId, key)
            .AddParameterWithValue(BlockedArgumentTable.Field.Enabled, enabled)
            .ExecuteListReading<BlockedArgument>(ReadBlockedArgument, action);
    }
}
