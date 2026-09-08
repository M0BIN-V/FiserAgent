using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Common.Extensions;
using Supervisor.Application.Services.Process;

namespace Supervisor.Infra.Services;

public sealed class ProcessManager(
    ProcessProfile profile,
    ILogger<ProcessManager> baseLogger,
    IPipeClient pipeClient) : IProcessManager
{
    public async Task<bool> IsRunningHealthyAsync(CancellationToken ct)
    {
        if (!IsProcessRunning()) return false;

        var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            await pipeClient.ConnectAsync(profile.PipeName, timeoutCts.Token);
        }
        catch (OperationCanceledException)
        {
            return false;
        }

        try
        {
            await pipeClient.PingAsync(ct);
        }
        catch
        {
            return false;
        }
        finally
        {
            await pipeClient.DisposeAsync();
        }

        return true;
    }

    public async Task<Process> StartProcess(
        string filePath,
        Dictionary<string, string> environmentVariables,
        List<string>? args = null,
        DataReceivedEventHandler? onOutput = null,
        DataReceivedEventHandler? onError = null,
        CancellationToken ct = default)
    {
        if (await IsRunningHealthyAsync(ct))
            throw new InvalidOperationException("Process is already running.");

        var process = InitProcess(filePath, environmentVariables, args ?? []);

        process.OutputDataReceived += onOutput;
        process.ErrorDataReceived += onError;

        if (!process.Start()) throw new InvalidOperationException($"Failed to start process : {process.ProcessName}");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        profile.ProcessName = process.ProcessName;
        profile.ProcessId = process.Id;

        return process;
    }

    public async Task ShutdownAsync(CancellationToken ct)
    {
        await pipeClient.ConnectAsync(profile.PipeName, ct);
        await pipeClient.ShutdownAsync(ct);
        await pipeClient.DisposeAsync();
    }

    private bool IsProcessRunning()
    {
        if (!profile.ProcessId.HasValue) return false;

        try
        {
            baseLogger.LogDebug($"Connecting to process : {profile.ProcessId}");
            using var process = Process.GetProcessById(profile.ProcessId.Value);

            return !process.HasExited &&
                   string.Equals(process.ProcessName, profile.ProcessName, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException e)
        {
            baseLogger.LogDebug(e.Message);
            return false;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            // Access denied
            baseLogger.LogDebug(ex.Message);
            return false;
        }
    }

    private Process InitProcess(
        string filePath,
        Dictionary<string, string> environmentVariables,
        List<string> args)
    {
        environmentVariables.Add("SUPERVISOR_PIPE_NAME", profile.PipeName);

        var startInfo = new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        args.ForEach(a => startInfo.ArgumentList.Add(a));

        foreach (var keyValuePair in environmentVariables) startInfo.Environment[keyValuePair.Key] = keyValuePair.Value;

        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        return process;
    }
}