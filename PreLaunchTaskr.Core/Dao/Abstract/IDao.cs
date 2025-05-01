using System;
using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public interface IDao<TData>
{
    public List<TData> List(int limit = -1, int offset = 0);

    /// <summary>
    /// 对每项数据执行操作。不同于先 List 再操作，这个操作不会创建完整的列表，而是在取到每个数据时就执行操作。
    /// </summary>
    /// <param name="action">要执行的操作</param>
    /// <returns>数据的数量</returns>
    public int ForEach(Action<TData> action, int limit = -1, int offset = 0);

    public int Clear();

    public void CreateTableIfNotExists();
}
