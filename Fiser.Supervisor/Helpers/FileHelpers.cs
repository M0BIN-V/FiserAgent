namespace Fiser.Supervisor.Helpers;

public static class FileHelpers
{
    public static async Task CopyDirectoryAsync(
        string sourceDirectory,
        string destinationDirectory,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException(sourceDirectory);

        Directory.CreateDirectory(destinationDirectory);

        var files = Directory.GetFiles(
            sourceDirectory,
            "*",
            SearchOption.AllDirectories);

        var totalBytes = files.Sum(x => new FileInfo(x).Length);
        long copiedBytes = 0;

        foreach (var sourceFile in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var relativePath = Path.GetRelativePath(
                sourceDirectory,
                sourceFile);

            var destinationFile = Path.Combine(
                destinationDirectory,
                relativePath);

            Directory.CreateDirectory(
                Path.GetDirectoryName(destinationFile)!);

            await using var sourceStream = new FileStream(
                sourceFile,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                1024 * 1024,
                true);

            await using var destinationStream = new FileStream(
                destinationFile,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                1024 * 1024,
                true);

            var buffer = new byte[1024 * 1024];

            int bytesRead;

            while ((bytesRead = await sourceStream.ReadAsync(
                       buffer,
                       cancellationToken)) > 0)
            {
                await destinationStream.WriteAsync(
                    buffer.AsMemory(0, bytesRead),
                    cancellationToken);

                copiedBytes += bytesRead;

                var percentage = totalBytes == 0 ? 100 : (double)copiedBytes / totalBytes * 100;

                progress?.Report(percentage);
            }
        }
    }
}