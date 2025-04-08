namespace PreLaunchTaskr.Core.Dao.Tables;

public abstract class AbstractTable<TSelf> where TSelf : AbstractTable<TSelf>, new()
{
    protected abstract string[] _AllFields { get; }
    protected abstract string[] _AllColumns { get; }

    protected static string GetColumnName(string table, string field) => $"{table}.{field}";

    /// <summary>
    /// 单例，属性以静态方式对外提供
    /// </summary>
    protected static readonly TSelf self = new();

    public static string[] AllFields => self._AllFields;
    public static string[] AllColumns => self._AllColumns;
}
