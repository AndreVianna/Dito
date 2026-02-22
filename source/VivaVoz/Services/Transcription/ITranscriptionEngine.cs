namespace VivaVoz.Services.Transcription;

public interface ITranscriptionEngine {
    Task<TranscriptionResult> TranscribeAsync(
        string audioFilePath,
        TranscriptionOptions options,
        CancellationToken cancellationToken = default);

    IReadOnlyList<string> SupportedLanguages { get; }
    bool IsAvailable { get; }
}
