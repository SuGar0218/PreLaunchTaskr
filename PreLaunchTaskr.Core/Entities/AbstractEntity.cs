using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Entities;

public abstract class AbstractEntity
{
    protected AbstractEntity(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}
