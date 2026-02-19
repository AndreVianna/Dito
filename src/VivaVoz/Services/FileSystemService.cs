using System;
using System.Diagnostics;
using System.IO;
using VivaVoz.Constants;

namespace VivaVoz.Services;

public sealed class FileSystemService
{
    public void EnsureAppDirectories()
    {
        var directories = new[]
        {
            FilePaths.AppDataDirectory,
            FilePaths.DataDirectory,
            FilePaths.AudioDirectory,
            FilePaths.ModelsDirectory,
            FilePaths.LogsDirectory
        };

        foreach (var directory in directories)
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogDirectoryError(directory, ex);
            }
            catch (IOException ex)
            {
                LogDirectoryError(directory, ex);
            }
        }
    }

    private static void LogDirectoryError(string directory, Exception exception)
    {
        var message = $"[FileSystemService] Failed to create directory '{directory}'.";
        Trace.TraceError("{0} {1}", message, exception);
        Console.Error.WriteLine($"{message} {exception}");
    }
}
