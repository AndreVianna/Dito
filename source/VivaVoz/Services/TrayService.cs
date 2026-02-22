using System.Diagnostics.CodeAnalysis;

namespace VivaVoz.Services;

[ExcludeFromCodeCoverage]
public class TrayService : ITrayService {
    public void SetState(TrayIconState state) { }
    public void ShowTranscriptionComplete(string? transcript) { }
    public void Dispose() { }

    public static string FormatTooltipText(string? transcript) {
        if (string.IsNullOrEmpty(transcript))
            return "VivaVoz — No speech detected.";

        if (transcript.Length <= 30)
            return $"VivaVoz — {transcript}";

        return $"VivaVoz — {transcript[..30]}...";
    }

    public static string GetTooltipForState(TrayIconState state) => state switch {
        TrayIconState.Recording => "VivaVoz — Recording...",
        TrayIconState.Transcribing => "VivaVoz — Transcribing...",
        _ => "VivaVoz"
    };
}
