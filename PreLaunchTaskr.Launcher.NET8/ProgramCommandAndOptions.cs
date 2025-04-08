using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PreLaunchTaskr.CLI.Common.Extensions;

namespace PreLaunchTaskr.Launcher.NET8;

internal partial class Program
{
    private static readonly RootCommand rootCommand = new();

    private static readonly Option<int> idOption = new(
        ["--id", "-i"],
        "要启动的程序的 ID")
    {
        Arity = ArgumentArity.OneOrMore
    };

    private static readonly Option<int> pathOption = new(
        ["--path", "-p"],
        "要启动的程序的路径")
    {
        Arity = ArgumentArity.OneOrMore
    };

    static Program()
    {
        rootCommand.AddOptions(idOption, pathOption);
    }
}
