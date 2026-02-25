using System.Text;

namespace VivaVoz.Services;

/// <summary>
/// Exports recording content (transcript text and audio files) to user-specified destinations.
/// </summary>
public class ExportService : IExportService {
    /// <inheritdoc />
    public async Task ExportTextAsync(string transcript, string destinationPath) {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);

        var dir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await File.WriteAllTextAsync(destinationPath, transcript ?? string.Empty, Encoding.UTF8);
        Log.Information("[ExportService] Transcript exported to: {Path}", destinationPath);
    }

    /// <inheritdoc />
    public async Task ExportAudioAsync(string sourceAudioPath, string destinationPath) {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceAudioPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);

        if (!File.Exists(sourceAudioPath))
            throw new FileNotFoundException("Source audio file not found.", sourceAudioPath);

        var dir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await Task.Run(() => File.Copy(sourceAudioPath, destinationPath, overwrite: true));
        Log.Information("[ExportService] Audio exported to: {Path}", destinationPath);
    }
}
