# Task 1.13: Settings Persistence

**Goal:** Implement a robust mechanism to store and retrieve application settings, ensuring user preferences persist between sessions.
**Part of:** Delivery 1b

## Context
While Delivery 1b focuses on core functionality, we need a way to remember user choices (e.g., "Use base model instead of tiny", "Save to D:\MyAudio", "Dark mode"). This task creates the backend for the Settings screen (Task 2.6) and ensures defaults are loaded correctly on first launch.

## Requirements

### Functional
- On first launch, the application must detect if settings exist. If not, create default settings.
- Settings must be stored in the SQLite database (`vivavoz.db`).
- Changes to settings (e.g., via future UI or code) must be persisted immediately.
- The application must start up using the stored settings.

### Technical
- **Entity:** `Settings` class (Id, WhisperModelSize, StoragePath, Theme, Language, AutoUpdate, HotkeyConfig).
- **Service:** `ISettingsService` with methods:
    - `Task<Settings> GetAsync();` (Singleton or scoped - likely scoped per operation, but settings are global).
    - `Task SaveAsync(Settings settings);`
- **Defaults:**
    - WhisperModelSize: `Tiny`
    - StoragePath: `%LOCALAPPDATA%/VivaVoz`
    - Theme: `System`
    - Language: `Auto`
    - AutoUpdate: `False`
- **Migration:** Ensure EF Core migration is applied if the schema changes.

## Acceptance Criteria (Verification Steps)

### Verify: First run initialization
- [ ] Ensure no `vivavoz.db` exists (or clear the `Settings` table).
- [ ] Launch the application or initialize `SettingsService`.
- [ ] Verify that a new `Settings` record is created in the database.
- [ ] Confirm that `WhisperModelSize` defaults to `Tiny`.
- [ ] Confirm that `StoragePath` defaults to the application's local app data path.

### Verify: Load existing settings
- [ ] Manually update the `Settings` table (or use a previous run) to set `WhisperModelSize` to `Base`.
- [ ] Restart the application or re-initialize `SettingsService`.
- [ ] Call `GetAsync()` and verify that the returned object has `WhisperModelSize` set to `Base`.

### Verify: Persist changes
- [ ] Start with default settings (`WhisperModelSize` = `Tiny`).
- [ ] Use `SettingsService` to update `WhisperModelSize` to `Small` and save.
- [ ] Restart the application or query the database directly.
- [ ] Verify that the `Settings` table now contains `WhisperModelSize` = `Small`.

### Verify: Concurrent access
- [ ] (Optional) Simulate two components accessing settings simultaneously.
- [ ] Update a setting in one component.
- [ ] Verify that the other component receives the updated value (either via re-query or notification event, depending on implementation).
