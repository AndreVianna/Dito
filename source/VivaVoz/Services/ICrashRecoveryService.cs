namespace VivaVoz.Services;

/// <summary>
/// Detects and manages orphaned in-progress recordings left behind after an unexpected app crash.
/// </summary>
public interface ICrashRecoveryService {
    /// <summary>
    /// Returns true when a recovery marker exists and the referenced audio file is still on disk.
    /// </summary>
    bool HasOrphan();

    /// <summary>
    /// Returns the absolute path of the orphaned audio file, or null if no orphan is detected.
    /// </summary>
    string? GetOrphanPath();

    /// <summary>
    /// Deletes the recovery marker, permanently dismissing the orphan notification.
    /// </summary>
    void Dismiss();
}
