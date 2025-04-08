using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao.Abstract;
using PreLaunchTaskr.Core.Dao.Tables;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Utils.SqlUtils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Dao;

public class PreLaunchTaskDao :
    AbstractSqliteDao,
    IDao<PreLaunchTask>,
    IDaoWithPrimaryKey<int, PreLaunchTask>,
    IDaoWithForeignKey<int, PreLaunchTask>
{
    public PreLaunchTaskDao(SqliteConnection connection) : base(connection)
    {
        CreateTableIfNotExists();
    }

    public int Add(PreLaunchTask data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        int inserted = new SqliteCommandTextBuilder()
            .InsertInto(PreLaunchTaskTable.Name, PreLaunchTaskTable.AllFields[1..])
            .Values("(" +
                $"${PreLaunchTaskTable.Field.ProgramId}, " +
                $"${PreLaunchTaskTable.Field.TaskPath}, " +
                $"${PreLaunchTaskTable.Field.AcceptProgramArgs}, " +
                $"${PreLaunchTaskTable.Field.IncludeAttachedArgs}, " +
                $"${PreLaunchTaskTable.Field.Enabled})")
            .GetWritingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(PreLaunchTaskTable.Field.TaskPath, data.TaskPath)
            .AddParameterWithValue(PreLaunchTaskTable.Field.AcceptProgramArgs, data.AcceptProgramArgs)
            .AddParameterWithValue(PreLaunchTaskTable.Field.IncludeAttachedArgs, data.IncludeAttachedArgs)
            .AddParameterWithValue(PreLaunchTaskTable.Field.Enabled, data.Enabled)
            .ExecuteNonQuery();

        if (inserted == 0)
            return -1;

        return new SqliteCommandTextBuilder()
            .Select(SqliteCommandTextBuilder.LastInsertRowId())
            .From(PreLaunchTaskTable.Name)
            .GetReadingExcutor(command)
            .ExecuteScalarReading<int>();
    }

    public int Clear()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(PreLaunchTaskTable.Name)
            .GetWritingExcutor(command)
            .ExecuteNonQuery();
    }

    public int ClearByForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(PreLaunchTaskTable.Name)
            .Where($"{PreLaunchTaskTable.Column.ProgramId} = ${PreLaunchTaskTable.Field.ProgramId}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.ProgramId, key)
            .ExecuteNonQuery();
    }

    public bool ExistsForeignKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(PreLaunchTaskTable.Name)
            .Where($"{PreLaunchTaskTable.Column.ProgramId} = ${PreLaunchTaskTable.Field.ProgramId}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.ProgramId, key)
            .ExecuteScalarReading<bool>();
    }

    public bool ExistsPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(PreLaunchTaskTable.Name)
            .Where($"{PreLaunchTaskTable.Column.Id} = ${PreLaunchTaskTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.Id, key)
            .ExecuteScalarReading<bool>();
    }

    public PreLaunchTask? GetByPrimaryKey(int key)
    {
        //if (!ExistsPrimaryKey(key))
        //    return null;

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select(PreLaunchTaskTable.AllColumns, ProgramTable.AllColumns)
            .From(PreLaunchTaskTable.Name, ProgramTable.Name)
            .Where($"{PreLaunchTaskTable.Column.Id} = ${PreLaunchTaskTable.Column.Id}")
            .And($"{PreLaunchTaskTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Column.Id, key)
            .ExecuteDataReading<PreLaunchTask>(ReadPreLaunchTask);
    }

    public IList<PreLaunchTask> List(int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(PreLaunchTaskTable.AllColumns, ProgramTable.AllColumns)
            .Where($"{PreLaunchTaskTable.Column.ProgramId} = {ProgramTable.Column.Id}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<PreLaunchTask>(ReadPreLaunchTask);
    }

    public IList<PreLaunchTask> ListByForeignKey(int key, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(PreLaunchTaskTable.AllColumns, ProgramTable.AllColumns)
            .From(PreLaunchTaskTable.Name, ProgramTable.Name)
            .Where($"{PreLaunchTaskTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{PreLaunchTaskTable.Column.ProgramId} = ${PreLaunchTaskTable.Field.ProgramId}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.ProgramId, key)
            .ExecuteListReading<PreLaunchTask>(ReadPreLaunchTask);
    }

    public IList<PreLaunchTask> ListEnabledByForeignKey(int key, bool enabled, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
            .Select(PreLaunchTaskTable.AllColumns, ProgramTable.AllColumns)
            .From(PreLaunchTaskTable.Name, ProgramTable.Name)
            .Where($"{PreLaunchTaskTable.Column.ProgramId} = {ProgramTable.Column.Id}")
            .And($"{PreLaunchTaskTable.Column.ProgramId} = ${PreLaunchTaskTable.Field.ProgramId}")
            .And($"{PreLaunchTaskTable.Column.Enabled} = ${PreLaunchTaskTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.ProgramId, key)
            .AddParameterWithValue(PreLaunchTaskTable.Field.Enabled, enabled)
            .ExecuteListReading<PreLaunchTask>(ReadPreLaunchTask);
    }

    public int RemoveByPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(PreLaunchTaskTable.Name)
            .Where($"{PreLaunchTaskTable.Column.Id} = ${PreLaunchTaskTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.Id, key)
            .ExecuteNonQuery();
    }

    public int Update(PreLaunchTask data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Update(PreLaunchTaskTable.Name)
            .Set(
                $"{PreLaunchTaskTable.Field.ProgramId} = ${PreLaunchTaskTable.Field.ProgramId}",
                $"{PreLaunchTaskTable.Field.TaskPath} = ${PreLaunchTaskTable.Field.TaskPath}",
                $"{PreLaunchTaskTable.Field.AcceptProgramArgs} = ${PreLaunchTaskTable.Field.AcceptProgramArgs}",
                $"{PreLaunchTaskTable.Field.IncludeAttachedArgs} = ${PreLaunchTaskTable.Field.IncludeAttachedArgs}",
                $"{PreLaunchTaskTable.Field.Enabled} = ${PreLaunchTaskTable.Field.Enabled}")
            .Where($"{PreLaunchTaskTable.Column.Id} = ${PreLaunchTaskTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(PreLaunchTaskTable.Field.ProgramId, data.ProgramInfo.Id)
            .AddParameterWithValue(PreLaunchTaskTable.Field.TaskPath, data.TaskPath)
            .AddParameterWithValue(PreLaunchTaskTable.Field.AcceptProgramArgs, data.AcceptProgramArgs)
            .AddParameterWithValue(PreLaunchTaskTable.Field.IncludeAttachedArgs, data.IncludeAttachedArgs)
            .AddParameterWithValue(PreLaunchTaskTable.Field.Enabled, data.Enabled)
            .AddParameterWithValue(PreLaunchTaskTable.Field.Id, data.Id)
            .ExecuteNonQuery();
    }

    private static PreLaunchTask ReadPreLaunchTask(SqliteDataReader reader)
    {
        return new PreLaunchTask(
            reader.GetInt32(0),
            new ProgramInfo(reader.GetInt32(1), reader.GetString(7), reader.GetBoolean(8)),
            reader.GetString(2),
            reader.GetBoolean(3),
            reader.GetBoolean(4),
            reader.GetBoolean(5));
    }

    public void CreateTableIfNotExists()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = ProgramTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
        command.CommandText = PreLaunchTaskTable.CreateIfNotExistsSQL;
        command.ExecuteNonQuery();
    }
}
