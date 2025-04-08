using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Dao.Abstract;

public interface IDao<TData>
{
    public IList<TData> List(int limit = -1, int offset = 0);

    public int Clear();

    public void CreateTableIfNotExists();
}
