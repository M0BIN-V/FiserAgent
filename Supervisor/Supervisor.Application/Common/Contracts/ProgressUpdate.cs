namespace Supervisor.Application.Common.Contracts;

public sealed record ProgressUpdate(
    long Current,
    long Total,
    string? Message = null)
{
    public double Percentage => Total <= 0 ? 0 : (double)Current / Total * 100;
}