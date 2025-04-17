using Microsoft.Data.Sqlite;

using PreLaunchTaskr.Core.Dao.Abstract;
using PreLaunchTaskr.Core.Dao.Tables;
using PreLaunchTaskr.Core.Entities;
using PreLaunchTaskr.Core.Utils.SqlUtils;

using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao;

public class ProgramInfoDao :
    AbstractSqliteDao,
    IDao<ProgramInfo>,
    IDaoWithPrimaryKey<int, ProgramInfo>,
    IDaoWithUniqueKey<string, ProgramInfo>
{
    public ProgramInfoDao(SqliteConnection connection) : base(connection)
    {
        CreateTableIfNotExists();
    }

    public int Clear()
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
                .DeleteFrom(ProgramTable.Name)
                .GetWritingExcutor(command)
                .ExecuteNonQuery();
    }

    public ProgramInfo? GetByPrimaryKey(int key)
    {
        //if (!ExistsPrimaryKey(key))
        //    return null;

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select(ProgramTable.AllColumns)
            .From(ProgramTable.Name)
            .Where($"{ProgramTable.Column.Id} = ${ProgramTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Id, key)
            .ExecuteDataReading<ProgramInfo>(ReadProgramInfo)!;
    }

    public IList<ProgramInfo> List(int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
                .Select(ProgramTable.AllColumns)
                .From(ProgramTable.Name);

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .ExecuteListReading<ProgramInfo>(ReadProgramInfo);
    }

    public IList<ProgramInfo> ListEnabled(bool enabled, int limit = -1, int offset = 0)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        SqliteCommandTextBuilder builder = new SqliteCommandTextBuilder()
                .Select(ProgramTable.AllColumns)
                .From(ProgramTable.Name)
                .Where($"{ProgramTable.Column.Enabled} = ${ProgramTable.Field.Enabled}");

        if (limit > -1)
            builder.Limit(limit, offset);

        return builder
            .GetReadingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Enabled, enabled)
            .ExecuteListReading<ProgramInfo>(ReadProgramInfo);
    }

    /// <summary>
    /// 不会处理其他表对此的外键引用，调用前应考虑。
    /// </summary>
    public int RemoveByPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(ProgramTable.Name)
            .Where($"{ProgramTable.Field.Id} = ${ProgramTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Id, key)
            .ExecuteNonQuery();
    }

    public int Update(ProgramInfo data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Update(ProgramTable.Name)
            .Set(
                $"{ProgramTable.Field.Path} = ${ProgramTable.Field.Path}",
                $"{ProgramTable.Field.Enabled} = ${ProgramTable.Field.Enabled}")
            .Where($"{ProgramTable.Field.Id} = ${ProgramTable.Field.Id}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Path, data.Path)
            .AddParameterWithValue(ProgramTable.Field.Enabled, data.Enabled)
            .AddParameterWithValue(ProgramTable.Field.Id, data.Id)
            .ExecuteNonQuery();
    }

    public ProgramInfo? GetByUniqueKey(string key)
    {
        //if (!ExistsUniqueKey(key))
        //    return null;

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select(ProgramTable.AllColumns)
            .From(ProgramTable.Name)
            .Where($"{ProgramTable.Column.Path} = ${ProgramTable.Field.Path}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Path, key)
            .ExecuteDataReading<ProgramInfo>(ReadProgramInfo)!;
    }

    /// <summary>
    /// 不会处理其他表对此的外键引用，调用前应考虑。
    /// </summary>
    public int RemoveByUniqueKey(string key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .DeleteFrom(ProgramTable.Name)
            .Where($"{ProgramTable.Column.Path} = ${ProgramTable.Field.Path}")
            .GetWritingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Path, key)
            .ExecuteNonQuery();
    }

    public int Add(ProgramInfo data)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        int inserted = new SqliteCommandTextBuilder()
            .InsertInto(ProgramTable.Name, ProgramTable.AllFields[1..])
            .Values($"(${ProgramTable.Field.Path}, ${ProgramTable.Field.Enabled})")
            .GetWritingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Path, data.Path)
            .AddParameterWithValue(ProgramTable.Field.Enabled, data.Enabled)
            .ExecuteNonQuery();

        if (inserted == 0)
            return -1;

        return new SqliteCommandTextBuilder()
            .Select(SqliteCommandTextBuilder.LastInsertRowId())
            .From(ProgramTable.Name)
            .GetReadingExcutor(command)
            .ExecuteScalarReading<int>();
    }

    public bool ExistsPrimaryKey(int key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(ProgramTable.Name)
            .Where($"{ProgramTable.Column.Id} = ${ProgramTable.Field.Id}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Id, key)
            .ExecuteScalarReading<bool>();
    }

    public bool ExistsUniqueKey(string key)
    {
        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        return new SqliteCommandTextBuilder()
            .Select1.From(ProgramTable.Name)
            .Where($"{ProgramTable.Column.Path} = ${ProgramTable.Field.Path}")
            .GetReadingExcutor(command)
            .AddParameterWithValue(ProgramTable.Field.Path, key)
            .ExecuteScalarReading<bool>();
    }

    private static ProgramInfo ReadProgramInfo(SqliteDataReader reader)
    {
        return new ProgramInfo(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetBoolean(2));
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
