using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Configurator.NET8.Helpers;

public class ConsoleHelper
{
    public static void PrintError(string content)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(content);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void PrintWarning(string content)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(content);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void PrintInfo(string content)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(content);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void PrintSuccess(string content)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(content);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}
