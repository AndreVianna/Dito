namespace VivaVoz.Services;

public enum TrayIconState {
    Idle,
    Recording,
    Transcribing
}

public interface ITrayService : IDisposable {
    void SetState(TrayIconState state);
    void ShowTranscriptionComplete(string? transcript);
}
