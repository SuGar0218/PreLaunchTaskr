using PreLaunchTaskr.CLI.Common.Extensions;

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Configurator.NET8;

internal partial class Program
{
    private static readonly RootCommand rootCommand = new();

    #region commands

    private static readonly Command listProgramCommand = new(
        "list-program",
        "列出已配置的程序");

    private static readonly Command addProgramCommand = new(
        "add-program",
        "添加要配置的程序");

    private static readonly Command removeProgramCommand = new(
        "remove-program",
        "移除已配置的程序");

    private static readonly Command enableProgramCommand = new(
        "enable-program",
        "启用配置的程序（需要操作注册表，需要以管理员终端运行）");

    private static readonly Command disableProgramCommand = new(
        "disable-program",
        "禁用配置的程序（需要操作注册表，需要以管理员终端运行）");



    private static readonly Command listAttachedArgumentCommand = new(
        "list-attached-argument",
        "列出对指定程序附加的参数");

    private static readonly Command addAttachedArgumentCommand = new(
        "add-attached-argument",
        "对指定程序附加参数");

    private static readonly Command removeAttachedArgumentCommand = new(
        "remove-attached-argument",
        "移除程序附加参数");

    private static readonly Command enableAttachedArgumentCommand = new(
        "enable-attached-argument",
        "启用附加的参数");

    private static readonly Command disableAttachedArgumentCommand = new(
        "disable-attached-argument",
        "禁用附加的参数");



    private static readonly Command listBlockedArgumentCommand = new(
        "list-blocked-argument",
        "列出对指定程序屏蔽的参数");

    private static readonly Command addBlockedArgumentCommand = new(
        "add-blocked-argument",
        "对指定程序屏蔽参数");

    private static readonly Command removeBlockedArgumentCommand = new(
        "remove-blocked-argument",
        "移除程序屏蔽参数");

    private static readonly Command enableBlockedArgumentCommand = new(
        "enable-blocked-argument",
        "启用屏蔽的参数");

    private static readonly Command disableBlockedArgumentCommand = new(
        "disable-blocked-argument",
        "禁用屏蔽的参数");



    private static readonly Command listPreLaunchTaskCommand = new(
        "list-prelaunch-task",
        "列出指定程序的启动前任务");

    private static readonly Command addPreLaunchTaskCommand = new(
        "add-prelaunch-task",
        "为指定程序添加启动前任务");

    private static readonly Command removePreLaunchTaskCommand = new(
        "remove-prelaunch-task",
        "为指定程序移除启动前任务");

    private static readonly Command enablePreLaunchTaskCommand = new(
        "enable-prelaunch-task",
        "启用某个启动前任务");

    private static readonly Command disablePreLaunchTaskCommand = new(
        "disable-prelaunch-task",
        "禁用某个启动前任务");



    private static readonly Command listEnvironmentVariableCommand = new(
        "list-environment-variable",
        "列出指定程序专属的环境变量");

    private static readonly Command addEnvironmentVariableCommand = new(
        "add-environment-variable",
        "为指定程序添加专属的环境变量");

    private static readonly Command removeEnvironmentVariableCommand = new(
        "remove-environment-variable",
        "为指定程序移除专属的环境变量");

    private static readonly Command enableEnvironmentVariableCommand = new(
        "enable-environment-variable",
        "为指定程序启用专属的环境变量");

    private static readonly Command disableEnvironmentVariableCommand = new(
        "disable-environment-variable",
        "为指定程序禁用专属的环境变量");

    #endregion

    #region options

    private static readonly Option<bool> silentOption = new(
        ["--silent", "-s"],
        "静默运行（不显示控制台窗口）")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<int[]> listAttachedArgumentProgramIdOption = new(
        "--program-id",
        "根据此程序ID列出为它附加的参数")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> listBlockedArgumentProgramIdOption = new(
        "--program-id",
        "根据此程序ID列出为它屏蔽的参数")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> listPreLaunchTaskProgramIdOption = new(
        "--program-id",
        "根据此程序ID列出为它启动前的任务")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> listEnvironmentVariableProgramIdOption = new(
        "--program-id",
        "根据此程序ID列出为它专属的环境变量")
    { Arity = ArgumentArity.OneOrMore };



    private static readonly Option<string[]> addProgramPathOption = new(
        ["--path", "-p"],
        "添加位于指定路径的程序")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<bool> addProgramPathEnableFlag = new(
        ["--enable", "-e"],
        "添加并启用")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<int[]> addAttachedArgumentProgramIdOption = new(
        "--program-id",
        "需要附加参数的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<string[]> addAttachedArgumentOption = new(
        "--argument",
        "需要附加的参数")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<bool> addAttachedArgumentEnableFlag = new(
        ["--enable", "-e"],
        "添加并启用")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<int[]> addBlockedArgumentProgramIdOption = new(
        "--program-id",
        "需要屏蔽参数的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<string[]> addBlockedArgumentOption = new(
        "--argument",
        "需要屏蔽的参数")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<bool> addBlockedArgumentEnableFlag = new(
        ["--enable", "-e"],
        "添加并启用")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<bool> addBlockedArgumentRegexFlag = new(
        ["--regex", "-r"],
        "此屏蔽参数用正则表达式")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<int[]> addPreLaunchTaskProgramIdOption = new(
        "--program-id",
        "需要添加启动前任务的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<string[]> addPreLaunchTaskPathOption = new(
        "--task-path",
        "启动前任务 (exe, bat, cmd, 诸如此类) 所在路径")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<bool> addPreLaunchTaskAcceptProgramArgsFlag = new(
        ["--enable", "-e"],
        "接受启用程序的参数")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<bool> addPreLaunchTaskIncludeAttachedArgsFlag = new(
        ["--enable", "-e"],
        "包含附加的参数")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<bool> addPreLaunchTaskEnableFlag = new(
        ["--enable", "-e"],
        "添加并启用")
    { Arity = ArgumentArity.ZeroOrOne };

    private static readonly Option<int[]> addEnvironmentVariableProgramIdOption = new(
        "--program-id",
        "需要添加专属环境变量的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<string> addEnvironmentVariableKeyOption = new(
        ["--key", "-k"],
        "环境变量的名称 (key)")
    { Arity = ArgumentArity.ExactlyOne };

    private static readonly Option<string[]> addEnvironmentVariableValueOption = new(
        ["--value", "-v"],
        "环境变量的值 (value)")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<bool> addEnvironmentVariableEnableFlag = new(
        ["--enable", "-e"],
        "添加并启用")
    { Arity = ArgumentArity.ZeroOrOne };



    private static readonly Option<int[]> removeProgramIdOption = new(
        ["--id", "-i"],
        "要移除的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> removeAttachedArgumentIdOption = new(
        ["--id", "-i"],
        "要移除的附加参数ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> removeBlockedArgumentIdOption = new(
        ["--id", "-i"],
        "要移除的屏蔽参数ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> removePreLaunchTaskIdOption = new(
        ["--id", "-i"],
        "要移除的启动前任务ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> removeEnvironmentVariableIdOption = new(
        ["--id", "-i"],
        "要移除的专属环境变量ID")
    { Arity = ArgumentArity.OneOrMore };



    private static readonly Option<int[]> enableProgramIdOption = new(
        ["--id", "-i"],
        "要启用的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> enableAttachedArgumentIdOption = new(
        ["--id", "-i"],
        "要启用的附加参数ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> enableBlockedArgumentIdOption = new(
        ["--id", "-i"],
        "要启用的屏蔽参数ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> enablePreLaunchTaskIdOption = new(
        ["--id", "-i"],
        "要启用的启动前任务ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> enableEnvironmentVariableIdOption = new(
        ["--id", "-i"],
        "要启用的专属环境变量ID")
    { Arity = ArgumentArity.OneOrMore };



    private static readonly Option<int[]> disableProgramIdOption = new(
        ["--id", "-i"],
        "要禁用的程序ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> disableAttachedArgumentIdOption = new(
        ["--id", "-i"],
        "要禁用的附加参数ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> disableBlockedArgumentIdOption = new(
        ["--id", "-i"],
        "要禁用的屏蔽参数ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> disablePreLaunchTaskIdOption = new(
        ["--id", "-i"],
        "要禁用的启动前任务ID")
    { Arity = ArgumentArity.OneOrMore };

    private static readonly Option<int[]> disableEnvironmentVariableIdOption = new(
        ["--id", "-i"],
        "要禁用的专属环境变量ID")
    { Arity = ArgumentArity.OneOrMore };

    #endregion

    static Program()
    {
        rootCommand
            .AddGlobalOptions(silentOption)
            .HandleOption(RunSilent, silentOption)
            .AddCommands(
            listProgramCommand.AddAliases("ls-prog")
                .HandleNoOption(ListPrograms),
            listAttachedArgumentCommand.AddAliases("ls-attach")
                .AddOptions(listAttachedArgumentProgramIdOption)
                .HandleOption(ListAttachedArguments, listAttachedArgumentProgramIdOption),
            listBlockedArgumentCommand.AddAliases("ls-block")
                .AddOptions(listBlockedArgumentProgramIdOption)
                .HandleOption(ListBlockedArguments, listBlockedArgumentProgramIdOption),
            listPreLaunchTaskCommand.AddAliases("ls-pretsk")
                .AddOptions(listPreLaunchTaskProgramIdOption)
                .HandleOption(ListPreLaunchTasks, listPreLaunchTaskProgramIdOption),
            listEnvironmentVariableCommand.AddAliases("ls-envvar")
                .AddOptions(listPreLaunchTaskProgramIdOption)
                .HandleOption(ListEnvironmentVariables, listEnvironmentVariableProgramIdOption),

            addProgramCommand.AddAliases("add-prog")
                .AddOptions(addProgramPathOption, addProgramPathEnableFlag)
                .HandleOption(AddProgram, addProgramPathOption, addProgramPathEnableFlag),
            addAttachedArgumentCommand.AddAliases("add-attach")
                .AddOptions(
                    addAttachedArgumentProgramIdOption,
                    addAttachedArgumentOption,
                    addAttachedArgumentEnableFlag)
                .HandleOption(AttachArgument,
                    addAttachedArgumentProgramIdOption,
                    addAttachedArgumentOption,
                    addAttachedArgumentEnableFlag),
            addBlockedArgumentCommand.AddAliases("add-block")
                .AddOptions(
                    addBlockedArgumentProgramIdOption,
                    addBlockedArgumentOption,
                    addBlockedArgumentEnableFlag,
                    addBlockedArgumentRegexFlag)
                .HandleOption(BlockArgument,
                    addBlockedArgumentProgramIdOption,
                    addBlockedArgumentOption,
                    addBlockedArgumentEnableFlag,
                    addBlockedArgumentRegexFlag),
            addPreLaunchTaskCommand.AddAliases("add-pretsk")
                .AddOptions(
                    addPreLaunchTaskProgramIdOption,
                    addPreLaunchTaskPathOption,
                    addPreLaunchTaskEnableFlag,
                    addPreLaunchTaskAcceptProgramArgsFlag,
                    addPreLaunchTaskIncludeAttachedArgsFlag)
                .HandleOption(AddPreLaunchTask,
                    addPreLaunchTaskProgramIdOption,
                    addPreLaunchTaskPathOption,
                    addPreLaunchTaskEnableFlag,
                    addPreLaunchTaskAcceptProgramArgsFlag,
                    addPreLaunchTaskIncludeAttachedArgsFlag),
            addEnvironmentVariableCommand.AddAliases("add-envvar")
                .AddOptions(
                    addEnvironmentVariableProgramIdOption,
                    addEnvironmentVariableKeyOption,
                    addEnvironmentVariableValueOption,
                    addEnvironmentVariableEnableFlag)
                .HandleOption(AddEnvironmentVariable,
                    addEnvironmentVariableProgramIdOption,
                    addEnvironmentVariableKeyOption,
                    addEnvironmentVariableValueOption,
                    addEnvironmentVariableEnableFlag),

            removeProgramCommand.AddAliases("rm-prog")
                .AddOptions(removeProgramIdOption)
                .HandleOption(RemoveProgram, removeProgramIdOption),
            removeAttachedArgumentCommand.AddAliases("rm-attach")
                .AddOptions(removeAttachedArgumentIdOption)
                .HandleOption(RemoveAttachedArgument, removeAttachedArgumentIdOption),
            removeBlockedArgumentCommand.AddAliases("rm-block")
                .AddOptions(removeBlockedArgumentIdOption)
                .HandleOption(RemoveBlockedArgument, removeBlockedArgumentIdOption),
            removePreLaunchTaskCommand.AddAliases("rm-pretsk")
                .AddOptions(removePreLaunchTaskIdOption)
                .HandleOption(RemovePreLaunchTask, removePreLaunchTaskIdOption),
            removeEnvironmentVariableCommand.AddAliases("rm-envvar")
                .AddOptions(removeEnvironmentVariableIdOption)
                .HandleOption(RemoveEnvironmentVariable, removeEnvironmentVariableIdOption),

            enableProgramCommand.AddAliases("en-prog")
                .AddOptions(enableProgramIdOption)
                .HandleOption(EnableProgram, enableProgramIdOption),
            enableAttachedArgumentCommand.AddAliases("en-attach")
                .AddOptions(enableAttachedArgumentIdOption)
                .HandleOption(EnableAttachedArgument, enableAttachedArgumentIdOption),
            enableBlockedArgumentCommand.AddAliases("en-block")
                .AddOptions(enableBlockedArgumentIdOption)
                .HandleOption(EnableBlockedArgument, enableBlockedArgumentIdOption),
            enablePreLaunchTaskCommand.AddAliases("en-pretsk")
                .AddOptions(enablePreLaunchTaskIdOption)
                .HandleOption(EnablePreLaunchTask, enablePreLaunchTaskIdOption),
            enableEnvironmentVariableCommand.AddAliases("en-envvar")
                .AddOptions(enableEnvironmentVariableIdOption)
                .HandleOption(EnableEnvironmentVariable, enableEnvironmentVariableIdOption),

            disableProgramCommand.AddAliases("dis-prog")
                .AddOptions(disableProgramIdOption)
                .HandleOption(DisableProgram, disableProgramIdOption),
            disableAttachedArgumentCommand.AddAliases("dis-attach")
                .AddOptions(disableAttachedArgumentIdOption)
                .HandleOption(DisableAttachedArgument, disableAttachedArgumentIdOption),
            disableBlockedArgumentCommand.AddAliases("dis-block")
                .AddOptions(disableBlockedArgumentIdOption)
                .HandleOption(DisableBlockedArgument, disableBlockedArgumentIdOption),
            disablePreLaunchTaskCommand.AddAliases("dis-pretsk")
                .AddOptions(disablePreLaunchTaskIdOption)
                .HandleOption(DisablePreLaunchTask, disablePreLaunchTaskIdOption),
            disableEnvironmentVariableCommand.AddAliases("dis-envvar")
                .AddOptions(disableEnvironmentVariableIdOption)
                .HandleOption(DisableEnvironmentVariable, disableEnvironmentVariableIdOption)
        );
    }
}
