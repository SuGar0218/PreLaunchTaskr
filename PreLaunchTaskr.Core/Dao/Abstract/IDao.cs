using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public interface IDao<TData>
{
    public IList<TData> List(int limit = -1, int offset = 0);

    public int Clear();

    public void CreateTableIfNotExists();
}
