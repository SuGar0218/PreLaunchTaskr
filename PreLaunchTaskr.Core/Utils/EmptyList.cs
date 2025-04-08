using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Utils;

public class EmptyList
{
    public static List<T> Of<T>() => new List<T>(0);
}
