namespace VivaVoz.Services.Transcription;

public record TranscriptionOptions(
    string? Language = null,
    string? ModelId = null
);
