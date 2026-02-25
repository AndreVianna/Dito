namespace VivaVoz.Services;

/// <summary>
/// Pure state-machine implementation of <see cref="ITrayIconService"/>.
/// <para>
/// State transitions are tracked internally and forwarded to an optional
/// <c>onStateChanged</c> callback so that callers (e.g. a production
/// <see cref="TrayService"/> wrapper) can update the actual tray icon without
/// coupling this class to Avalonia.
/// </para>
/// </summary>
public sealed class TrayIconService : ITrayIconService {
    private readonly Action<AppState>? _onStateChanged;
    private CancellationTokenSource? _revertCts;

    /// <param name="onStateChanged">
    /// Optional callback invoked every time the state changes.
    /// Called on the same thread that called <see cref="SetState"/> or
    /// <see cref="SetStateTemporary"/>; for the revert call it is invoked on a
    /// thread-pool thread.
    /// </param>
    public TrayIconService(Action<AppState>? onStateChanged = null) {
        _onStateChanged = onStateChanged;
    }

    /// <inheritdoc/>
    public AppState CurrentState { get; private set; } = AppState.Idle;

    /// <inheritdoc/>
    public void SetState(AppState state) {
        CancelPendingRevert();
        ApplyState(state);
    }

    /// <inheritdoc/>
    public void SetStateTemporary(AppState state, TimeSpan duration) {
        CancelPendingRevert();
        ApplyState(state);

        var cts = new CancellationTokenSource();
        _revertCts = cts;

        _ = RevertToIdleAsync(duration, cts.Token);
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private void ApplyState(AppState state) {
        CurrentState = state;
        _onStateChanged?.Invoke(state);
    }

    private void CancelPendingRevert() {
        var cts = _revertCts;
        _revertCts = null;
        cts?.Cancel();
        cts?.Dispose();
    }

    private async Task RevertToIdleAsync(TimeSpan duration, CancellationToken token) {
        try {
            await Task.Delay(duration, token);
            ApplyState(AppState.Idle);
        }
        catch (OperationCanceledException) {
            // Cancelled by a subsequent SetState / SetStateTemporary call — intentional.
        }
    }
}
