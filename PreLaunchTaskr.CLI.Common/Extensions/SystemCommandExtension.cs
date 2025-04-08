using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.CLI.Common.Extensions;

public static class SystemCommandExtension
{
    public static Command AddCommands(this Command command, List<Command> subcommands)
    {
        foreach (Command subcommand in subcommands)
        {
            command.AddCommand(subcommand);
        }
        return command;
    }

    public static Command AddCommands(this Command command, params Command[] subcommands)
    {
        foreach (Command subcommand in subcommands)
        {
            command.AddCommand(subcommand);
        }
        return command;
    }

    public static Command AddOptions(this Command command, List<Option> options)
    {
        foreach (Option option in options)
        {
            command.AddOption(option);
        }
        return command;
    }

    public static Command AddOptions(this Command command, params Option[] options)
    {
        foreach (Option option in options)
        {
            command.AddOption(option);
        }
        return command;
    }

    public static Command AddGlobalOptions(this Command command, List<Option> options)
    {
        foreach (Option option in options)
        {
            command.AddGlobalOption(option);
        }
        return command;
    }

    public static Command AddGlobalOptions(this Command command, params Option[] options)
    {
        foreach (Option option in options)
        {
            command.AddGlobalOption(option);
        }
        return command;
    }

    public static Command AddAliases(this Command command, params string[] aliases)
    {
        foreach (string alias in aliases)
        {
            command.AddAlias(alias);
        }
        return command;
    }

    public static Command AddAliases(this Command command, List<string> aliases)
    {
        foreach (string alias in aliases)
        {
            command.AddAlias(alias);
        }
        return command;
    }

    public static Command HandleNoOption(this Command command,  Action action)
    {
        command.SetHandler(action);
        return command;
    }

    public static Command HandleOption<T>(
        this Command command,
        Action<T> action,
        Option<T> option)
    {
        command.SetHandler<T>(action, option);
        return command;
    }

    public static Command HandleOption<T1, T2>(
        this Command command,
        Action<T1, T2> action,
        Option<T1> option1, Option<T2> option2)
    {
        command.SetHandler<T1, T2>(action, option1, option2);
        return command;
    }

    public static Command HandleOption<T1, T2, T3>(
        this Command command,
        Action<T1, T2, T3> action,
        Option<T1> option1, Option<T2> option2, Option<T3> option3)
    {
        command.SetHandler<T1, T2, T3>(action, option1, option2, option3);
        return command;
    }

    public static Command HandleOption<T1, T2, T3, T4>(
        this Command command,
        Action<T1, T2, T3, T4> action,
        Option<T1> option1, Option<T2> option2, Option<T3> option3, Option<T4> option4)
    {
        command.SetHandler<T1, T2, T3, T4>(action, option1, option2, option3, option4);
        return command;
    }

    public static Command HandleOption<T1, T2, T3, T4, T5>(
        this Command command,
        Action<T1, T2, T3, T4, T5> action,
        Option<T1> option1, Option<T2> option2, Option<T3> option3, Option<T4> option4, Option<T5> option5)
    {
        command.SetHandler<T1, T2, T3, T4, T5>(action, option1, option2, option3, option4, option5);
        return command;
    }

    public static Command HandleOption<T1, T2, T3, T4, T5, T6>(
        this Command command,
        Action<T1, T2, T3, T4, T5, T6> action,
        Option<T1> option1, Option<T2> option2, Option<T3> option3, Option<T4> option4, Option<T5> option5, Option<T6> option6)
    {
        command.SetHandler<T1, T2, T3, T4, T5, T6>(action, option1, option2, option3, option4, option5, option6);
        return command;
    }
}
