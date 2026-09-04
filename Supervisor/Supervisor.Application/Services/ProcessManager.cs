using System.ComponentModel;
using System.Diagnostics;
using Supervisor.Application.Services.ProcessProfile;

namespace Supervisor.Application.Services;

public abstract class ProcessManager<TProcessProfile>(
    PipeClient pipeClient,
    ProfileService<TProcessProfile> profileService)
    : IDisposable
    where TProcessProfile : ProcessProfile.ProcessProfile, new()
{
    protected readonly ProfileService<TProcessProfile> ProfileService = profileService;
    protected Process? Process;
    protected abstract string FilePath { get; set; }

    public void Dispose()
    {
        Process?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected abstract void OnOutput(object? sender, DataReceivedEventArgs e);
    protected abstract void OnError(object? sender, DataReceivedEventArgs e);

    protected static bool IsProcessRunning(TProcessProfile profile)
    {
        try
        {
            using var process = Process.GetProcessById(profile.ProcessId);

            return !process.HasExited && string.Equals(
                process.ProcessName,
                profile.ProcessName,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            // Access denied
            return false;
        }
    }

    public virtual async Task<bool> IsRunningHealthyAsync(CancellationToken ct)
    {
        if (!ProfileService.ProfileExists()) return false;

        var profile = await ProfileService.GetProfileAsync(ct);

        var processIsRunning = IsProcessRunning(profile);

        if (!processIsRunning) return false;

        var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            await pipeClient.ConnectAsync(profile.PipeName, timeoutCts.Token);
        }
        catch (OperationCanceledException e)
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

        return processIsRunning;
    }

    protected async Task StartProcess(CancellationToken ct)
    {
        if (await IsRunningHealthyAsync(ct)) throw new InvalidOperationException("Process is already running.");

        var pipeName = Guid.NewGuid().ToString("N");

        InitProcess(pipeName);

        if (!Process!.Start()) throw new InvalidOperationException($"Failed to start process : {FilePath}");

        TProcessProfile profile;

        if (ProfileService.ProfileExists()) profile = await ProfileService.GetProfileAsync(ct);
        else profile = new TProcessProfile();

        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();

        profile.PipeName = pipeName;
        profile.ProcessName = Process.ProcessName;
        profile.ProcessId = Process.Id;

        await ProfileService.UpdateProfileAsync(profile, ct);
    }

    public async Task ShutdownAsync(CancellationToken ct)
    {
        var profile = await ProfileService.GetProfileAsync(ct);
        await pipeClient.ConnectAsync(profile.PipeName, ct);

        await pipeClient.ShutdownAsync(ct);
        await pipeClient.DisposeAsync();
    }

    private void InitProcess(string pipeName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = FilePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            Environment =
            {
                ["SUPERVISOR_PIPE_NAME"] = pipeName
            }
        };

        Process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        Process.OutputDataReceived += OnOutput;
        Process.ErrorDataReceived += OnError;
    }
}