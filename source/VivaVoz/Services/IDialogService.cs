namespace VivaVoz.Services;

/// <summary>
/// Abstraction over system dialogs for testability.
/// </summary>
public interface IDialogService {
    /// <summary>
    /// Shows a confirmation dialog and returns true if the user confirmed.
    /// </summary>
    Task<bool> ShowConfirmAsync(string title, string message);

    /// <summary>
    /// Shows a native Save File dialog and returns the chosen path, or null if cancelled.
    /// </summary>
    Task<string?> ShowSaveFileDialogAsync(
        string title,
        string suggestedFileName,
        string defaultExtension,
        string filterName,
        string[] filterPatterns);
}
