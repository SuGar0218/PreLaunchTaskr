using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public interface IDaoWithForeignKey<TForeignKey, TData>
{
    public IList<TData> ListByForeignKey(TForeignKey key, int limit = -1, int offset = 0);

    public bool ExistsForeignKey(TForeignKey key);

    /// <summary>
    /// 注意：不会处理其他表对此的外键引用，调用前应考虑。
    /// </summary>
    public int ClearByForeignKey(TForeignKey key);
}
