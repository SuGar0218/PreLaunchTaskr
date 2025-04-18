using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

public class XamlHelper
{
    public static bool And(bool b1, bool b2) => b1 && b2;
    public static bool Or(bool b1, bool b2) => b1 || b2;
    public static bool Not(bool b) => !b;
}
