using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public interface IDaoWithPrimaryKey<TPrimaryKey, TData> where TPrimaryKey : notnull
{
    public TData? GetByPrimaryKey(TPrimaryKey key);

    public bool ExistsPrimaryKey(TPrimaryKey key);

    public TPrimaryKey Add(TData data);

    public int Update(TData data);

    /// <summary>
    /// 注意：不会处理其他表对此的外键引用，调用前应考虑。
    /// </summary>
    public int RemoveByPrimaryKey(TPrimaryKey key);
}
