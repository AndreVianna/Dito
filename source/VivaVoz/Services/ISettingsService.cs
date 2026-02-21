namespace VivaVoz.Services;

/// <summary>
/// Service for loading and persisting application settings.
/// Settings are stored in the SQLite database and loaded on startup.
/// </summary>
public interface ISettingsService {
    /// <summary>
    /// Gets the current cached settings. Returns null if not yet loaded.
    /// Call <see cref="LoadSettingsAsync"/> first to ensure settings are available.
    /// </summary>
    Settings? Current { get; }

    /// <summary>
    /// Loads settings from the database. If no settings exist, creates and persists defaults.
    /// </summary>
    /// <returns>The loaded or newly created settings.</returns>
    Task<Settings> LoadSettingsAsync();

    /// <summary>
    /// Persists the given settings to the database. Updates the cached <see cref="Current"/> value.
    /// </summary>
    /// <param name="settings">The settings to save.</param>
    Task SaveSettingsAsync(Settings settings);
}
