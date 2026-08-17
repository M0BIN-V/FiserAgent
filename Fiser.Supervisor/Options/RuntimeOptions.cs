namespace Fiser.Supervisor.Options;

public class RuntimeOptions
{
    public string FilePath = Path.Combine(AppContext.BaseDirectory, "Runtime", "Fiser.Runtime.exe");
    public string FolderPath = Path.Combine(AppContext.BaseDirectory, "Runtime");
    public string ManifestPath = Path.Combine(AppContext.BaseDirectory, "Runtime", "runtime.json");
}