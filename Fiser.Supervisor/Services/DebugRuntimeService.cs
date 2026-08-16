using Fiser.Supervisor.Helpers;
using Fiser.Supervisor.Options;
using Microsoft.Extensions.Options;

namespace Fiser.Supervisor.Services;

public class DebugRuntimeService(
    IOptions<RuntimeOptions> runtimeOptions,
    IOptions<SupervisorOptions> supervisorOptions) : IRuntimeService
{
    private readonly RuntimeOptions _runtimeOptions = runtimeOptions.Value;
    private readonly SupervisorOptions _supervisorOptions = supervisorOptions.Value;

    public async Task<Version?> GetRuntimeVersionAsync()
    {
        throw new NotImplementedException();
    }

    public Version GetLatestRuntimeVersion()
    {
        throw new NotImplementedException();
    }

    public bool RunIsTimeInstalled()
    {
        var runtimeFolder = _runtimeOptions.FolderPath;

        if (!Directory.Exists(runtimeFolder))
            return false;

        var runtimeFile = _runtimeOptions.FilePath;

        if (!File.Exists(runtimeFile))
            return false;

        return true;
    }

    public async Task FetchRuntimeAsync(IProgress<double> progress)
    {
        //Copy runtime exe file from project 
        var runtimeFolder = _runtimeOptions.FolderPath;
        var supervisorProjectPath = Path.Combine(_supervisorOptions.Directory, "..", "..","..");
        var runtimeBuildFolder = Path.Combine(
            supervisorProjectPath,
            "..",
            "Fiser.Runtime",
            "Fiser.Runtime.WebApi",
            "bin",
            "Debug",
            "net10.0");

        await FileHelpers.CopyDirectoryAsync(runtimeBuildFolder, runtimeFolder, progress);
    }
}