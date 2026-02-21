namespace VivaVoz.Services.Transcription;

public sealed class TranscriptionCompletedEventArgs(
    Guid recordingId,
    bool success,
    string? transcript,
    string? detectedLanguage,
    string? modelUsed,
    string? errorMessage) : EventArgs {

    public Guid RecordingId { get; } = recordingId;
    public bool Success { get; } = success;
    public string? Transcript { get; } = transcript;
    public string? DetectedLanguage { get; } = detectedLanguage;
    public string? ModelUsed { get; } = modelUsed;
    public string? ErrorMessage { get; } = errorMessage;

    public static TranscriptionCompletedEventArgs Succeeded(
        Guid recordingId, string transcript, string detectedLanguage, string modelUsed)
        => new(recordingId, true, transcript, detectedLanguage, modelUsed, null);

    public static TranscriptionCompletedEventArgs Failed(Guid recordingId, string errorMessage)
        => new(recordingId, false, null, null, null, errorMessage);
}
