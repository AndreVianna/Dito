namespace VivaVoz.Services;

/// <summary>
/// Service for exporting recording content (transcript text and audio file) to user-chosen locations.
/// </summary>
public interface IExportService {
    /// <summary>
    /// Writes the given transcript text to the specified destination path.
    /// </summary>
    Task ExportTextAsync(string transcript, string destinationPath);

    /// <summary>
    /// Copies the source audio file to the specified destination path.
    /// </summary>
    Task ExportAudioAsync(string sourceAudioPath, string destinationPath);
}
