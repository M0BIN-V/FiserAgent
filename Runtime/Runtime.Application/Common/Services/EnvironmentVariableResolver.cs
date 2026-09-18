using System.Text.RegularExpressions;

namespace Runtime.Application.Common.Services;

public static partial class EnvironmentVariableResolver
{
    [GeneratedRegex(@"\$\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\}")]
    private static partial Regex EnvironmentVariableRegex();

    public static string Resolve(string value)
    {
        return EnvironmentVariableRegex().Replace(
            value,
            match =>
            {
                var name = match.Groups["name"].Value;

                return Environment.GetEnvironmentVariable(name)
                       ?? throw new InvalidOperationException(
                           $"Environment variable '{name}' is not defined.");
            });
    }

    public static Dictionary<string, string> Resolve(
        IReadOnlyDictionary<string, string> values)
    {
        return values.ToDictionary(
            x => x.Key,
            x => Resolve(x.Value));
    }
}