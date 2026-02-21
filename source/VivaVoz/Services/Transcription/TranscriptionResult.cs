namespace VivaVoz.Services.Transcription;

public record TranscriptionResult(
    string Text,
    string DetectedLanguage,
    TimeSpan Duration,
    string ModelUsed,
    float? Confidence = null
);
