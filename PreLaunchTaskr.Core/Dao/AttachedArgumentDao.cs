using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao.Abstract;
using PreLaunchTaskr.Core.Dao.Tables;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Utils.SqlUtils;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao;

public class AttachedArgumentDao :
    AbstractSqliteDao,
    IDao<AttachedArgument>,
    IDaoWithPrimaryKey<int, AttachedArgument>,
    IDaoWithForeignKey<int, AttachedArgument>
{
    public AttachedArgumentDao(SqliteConnection connection) : base(connection)
    {
        CreateTableIfNotExists();
    }

    public int Clear()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(AttachedArgumentTable.Name)
            .GetWritingExcutor(command)
            .ExecuteNonQuery();
    }

    public int ClearByForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(AttachedArgumentTable.Name)
            .Where($"{AttachedArgumentTable.Column.ProgramId} = ${AttachedArgumentTable.Field.ProgramId}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.ProgramId, key)
            .ExecuteNonQuery();
    }

    public AttachedArgument? GetByPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select(AttachedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(AttachedArgumentTable.Name, ProgramTable.Name)
            .Where($"{AttachedArgumentTable.Column.Id} = ${AttachedArgumentTable.Field.Id}")
            .And($"{AttachedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.Id, key)
            .ExecuteDataReading<AttachedArgument>(ReadAttachedArgument)!;
    }

    public IList<AttachedArgument> List(int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(AttachedArgumentTable.AllColumns)
            .From(AttachedArgumentTable.Name, ProgramTable.Name)
            .Where($"{AttachedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<AttachedArgument>(ReadAttachedArgument);
    }

    public IList<AttachedArgument> ListByForeignKey(int key, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(AttachedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(AttachedArgumentTable.Name, ProgramTable.Name)
            .Where($"{AttachedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{AttachedArgumentTable.Column.ProgramId} = ${AttachedArgumentTable.Field.ProgramId}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.ProgramId, key)
            .ExecuteListReading<AttachedArgument>(ReadAttachedArgument);
    }

    public IList<AttachedArgument> ListEnabledByForeignKey(int key, bool enabled, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(AttachedArgumentTable.AllColumns, ProgramTable.AllColumns)
            .From(AttachedArgumentTable.Name, ProgramTable.Name)
            .Where($"{AttachedArgumentTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{AttachedArgumentTable.Column.ProgramId} = ${AttachedArgumentTable.Field.ProgramId}")
            .And($"{AttachedArgumentTable.Column.Enabled} = ${AttachedArgumentTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.ProgramId, key)
            .AddParameterWithValue(AttachedArgumentTable.Field.Enabled, enabled)
            .ExecuteListReading<AttachedArgument>(ReadAttachedArgument);
    }

    public int RemoveByPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(AttachedArgumentTable.Name)
            .Where($"{AttachedArgumentTable.Column.Id} = ${AttachedArgumentTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.Id, key)
            .ExecuteNonQuery();
    }

    public int Update(AttachedArgument data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Update(AttachedArgumentTable.Name)
            .Set(
                $"{AttachedArgumentTable.Field.ProgramId} = ${AttachedArgumentTable.Field.ProgramId}",
                $"{AttachedArgumentTable.Field.Argument} = ${AttachedArgumentTable.Field.Argument}",
                $"{AttachedArgumentTable.Field.Enabled} = ${AttachedArgumentTable.Field.Enabled}")
            .Where($"{AttachedArgumentTable.Column.Id} = ${AttachedArgumentTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(AttachedArgumentTable.Field.Argument, data.Argument)
            .AddParameterWithValue(AttachedArgumentTable.Field.Enabled, data.Enabled)
            .AddParameterWithValue(AttachedArgumentTable.Field.Id, data.Id)
            .ExecuteNonQuery();
    }

    public bool ExistsPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(AttachedArgumentTable.Name)
            .Where($"{AttachedArgumentTable.Column.Id} = ${AttachedArgumentTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.Id, key)
            .ExecuteScalarReading<bool>();
    }

    public bool ExistsForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(AttachedArgumentTable.Name)
            .Where($"{AttachedArgumentTable.Column.ProgramId} = ${AttachedArgumentTable.Field.ProgramId}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.Id, key)
            .ExecuteScalarReading<bool>();
    }

    public int Add(AttachedArgument data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        int inserted = new SqliteCommandTextBuilder()
            .InsertInto(AttachedArgumentTable.Name, AttachedArgumentTable.AllFields[1..])
            .Values($"(${AttachedArgumentTable.Field.ProgramId}, ${AttachedArgumentTable.Field.Argument}, ${AttachedArgumentTable.Field.Enabled})")
            .GetWritingExcutor(command)
            .AddParameterWithValue(AttachedArgumentTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(AttachedArgumentTable.Field.Argument, data.Argument)
            .AddParameterWithValue(AttachedArgumentTable.Field.Enabled, data.Enabled)
            .ExecuteNonQuery();

        if (inserted == 0)
            return -1;

        return new SqliteCommandTextBuilder()
            .Select(SqliteCommandTextBuilder.LastInsertRowId())
            .From(AttachedArgumentTable.Name)
            .GetReadingExcutor(command)
            .ExecuteScalarReading<int>();
    }

    private static AttachedArgument ReadAttachedArgument(SqliteDataReader reader)
    {
        return new AttachedArgument(
            reader.GetInt32(0),
            new ProgramInfo(reader.GetInt32(1), reader.GetString(5), reader.GetBoolean(6)),
            reader.GetString(2),
            reader.GetBoolean(3));
    }

    public void CreateTableIfNotExists()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = ProgramTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
        command.CommandText = AttachedArgumentTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
    }
}
