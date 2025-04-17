namespace PreLaunchTaskr.Core.Dao.Abstract;

public interface IDaoWithUniqueKey<TUniqueKey, TData>
{
    public TData? GetByUniqueKey(TUniqueKey key);

    public bool ExistsUniqueKey(TUniqueKey key);

    /// <summary>
    /// 注意：不会处理其他表对此的外键引用，调用前应考虑。
    /// </summary>
    public int RemoveByUniqueKey(TUniqueKey key);
}
