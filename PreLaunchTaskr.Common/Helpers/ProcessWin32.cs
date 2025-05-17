using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.System.Threading;
using Windows.Win32.Foundation;

namespace PreLaunchTaskr.Common.Helpers;

public class ProcessWin32 : IDisposable
{
    private ProcessWin32(PROCESS_INFORMATION info)
    {
        this.info = info;
    }

    private readonly PROCESS_INFORMATION info;

    public uint ProcessId => info.dwProcessId;
    public uint ThreadId => info.dwThreadId;

    public static ProcessWin32? CreateSkipDebugger(string cmd, IDictionary<string, string>? environment = null, string? directory = null)
    {
        Span<char> cmdCharArray = (cmd + '\0').ToCharArray();
        directory ??= Path.GetDirectoryName(StringHelper.GetFirstToken(cmd));
        unsafe
        {
            fixed (char* lpEnvironment = environment is null ? null : BuildEnvironmentString(environment).ToCharArray())
            {
                ProcessWin32? process = CreateSuspended(cmd, environment, directory);
                if (process is not null)
                {
                    process.StopDebug();
                    process.Resume();
                }
                return process;
            }
        }
    }

    public static ProcessWin32? CreateAndStart(string cmd, IDictionary<string, string>? environment = null, string? directory = null)
    {
        Span<char> cmdCharArray = (cmd + '\0').ToCharArray();
        directory ??= Path.GetDirectoryName(StringHelper.GetFirstToken(cmd));
        unsafe
        {
            fixed (char* lpEnvironment = environment is null ? null : BuildEnvironmentString(environment).ToCharArray())
            {
                return PInvoke.CreateProcess(
                    null,
                    ref cmdCharArray,
                    null,
                    null,
                    false,
                    0,
                    lpEnvironment,
                    directory,
                    new STARTUPINFOW(),
                    out PROCESS_INFORMATION info) ? new ProcessWin32(info) : null;
            }
        }
    }

    public static ProcessWin32? CreateSuspended(string cmd, IDictionary<string, string>? environment = null, string? directory = null)
    {
        Span<char> cmdCharArray = (cmd + '\0').ToCharArray();
        directory ??= Path.GetDirectoryName(StringHelper.GetFirstToken(cmd));
        unsafe
        {
            fixed (char* lpEnvironment = environment is null ? null : BuildEnvironmentString(environment).ToCharArray())
            {
                return PInvoke.CreateProcess(
                    null,
                    ref cmdCharArray,
                    null,
                    null,
                    false,
                    PROCESS_CREATION_FLAGS.DEBUG_ONLY_THIS_PROCESS | PROCESS_CREATION_FLAGS.CREATE_SUSPENDED,
                    lpEnvironment,
                    directory,
                    new STARTUPINFOW(),
                    out PROCESS_INFORMATION info) ? new ProcessWin32(info) : null;
            }
        }
    }

    public bool StopDebug()
    {
        return PInvoke.DebugActiveProcessStop(ProcessId);
    }

    public bool Resume()
    {
        return PInvoke.ResumeThread(info.hThread) != uint.MaxValue;
    }

    public bool Suspend()
    {
        return PInvoke.SuspendThread(info.hThread) != uint.MaxValue;
    }

    private static string BuildEnvironmentString(IDictionary<string, string> environment)
    {
        StringBuilder stringBuilder = new();
        foreach (string key in environment.Keys)
        {
            stringBuilder.Append($"{key}={environment[key]}").Append('\0');
        }
        return stringBuilder.Append('\0').ToString();
    }

    public void Dispose()
    {
        PInvoke.CloseHandle(info.hThread);
        PInvoke.CloseHandle(info.hProcess);
    }
}
