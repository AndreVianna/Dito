# Task 1.13: Settings Persistence

**Status:** ✅ Complete
**Goal:** Implement a robust mechanism to store and retrieve application settings, ensuring user preferences persist between sessions.
**Part of:** Delivery 1b (final task)

## Context
While Delivery 1b focuses on core functionality, we need a way to remember user choices (e.g., "Use base model instead of tiny", "Save to D:\MyAudio", "Dark mode"). This task creates the backend for the Settings screen (Task 2.6) and ensures defaults are loaded correctly on first launch.

## Requirements

### Functional
- On first launch, the application must detect if settings exist. If not, create default settings.
- Settings must be stored in the SQLite database (`vivavoz.db`).
- Changes to settings (e.g., via future UI or code) must be persisted immediately.
- The application must start up using the stored settings.

### Technical
- **Entity:** `Settings` class (Id, WhisperModelSize, StoragePath, Theme, Language, AutoUpdate, HotkeyConfig, AudioInputDevice, ExportFormat).
- **Interface:** `ISettingsService` with:
    - `Settings? Current { get; }` — cached settings (null until loaded)
    - `Task<Settings> LoadSettingsAsync();` — loads from DB or creates defaults
    - `Task SaveSettingsAsync(Settings settings);` — persists changes
- **Implementation:** `SettingsService` using DbContext factory pattern (scoped context per operation)
- **Defaults:**
    - WhisperModelSize: `tiny`
    - StoragePath: `%LOCALAPPDATA%/VivaVoz`
    - Theme: `System`
    - Language: `auto`
    - ExportFormat: `MP3`
    - HotkeyConfig: (empty)
    - AudioInputDevice: null
    - AutoUpdate: `false`

## Implementation

### Files Created
- `source/VivaVoz/Services/ISettingsService.cs` — interface
- `source/VivaVoz/Services/SettingsService.cs` — EF Core-backed implementation
- `source/VivaVoz.Tests/Services/SettingsServiceTests.cs` — 14 unit tests

### Files Modified
- `source/VivaVoz/App.axaml.cs` — loads settings on startup, passes to MainViewModel
- `source/VivaVoz/ViewModels/MainViewModel.cs` — accepts ISettingsService, uses settings for recording defaults (WhisperModel, Language)

### Design Decisions
- **DbContext factory pattern:** SettingsService takes `Func<AppDbContext>` to avoid long-lived DbContext issues. Each load/save creates a scoped context.
- **Optional parameter in MainViewModel:** `ISettingsService? settingsService = null` with fallback to preserve backward compatibility with existing tests and constructors.
- **`Current` property:** Provides cached access to loaded settings without async overhead for code that needs settings synchronously (e.g., recording creation).
- **No migration needed:** Settings table already existed from Task 1.2. No schema changes.

## Acceptance Criteria (Verification Steps)

### ✅ Verify: First run initialization
- [x] Ensure no `vivavoz.db` exists (or clear the `Settings` table).
- [x] Initialize `SettingsService` via LoadSettingsAsync.
- [x] Verify that a new `Settings` record is created in the database.
- [x] Confirm that `WhisperModelSize` defaults to `tiny`.
- [x] Confirm that `StoragePath` defaults to the application's local app data path.

### ✅ Verify: Load existing settings
- [x] Pre-seed the `Settings` table with `WhisperModelSize` = `base`.
- [x] Initialize `SettingsService` via LoadSettingsAsync.
- [x] Verify that the returned object has `WhisperModelSize` set to `base`.

### ✅ Verify: Persist changes
- [x] Start with default settings (`WhisperModelSize` = `tiny`).
- [x] Use `SettingsService` to update `WhisperModelSize` to `small` and save.
- [x] Verify via fresh context that the `Settings` table now contains `WhisperModelSize` = `small`.

### ✅ Verify: No duplication
- [x] Load defaults, modify, save. Verify only one settings row exists.

## Test Summary
- 14 unit tests, all passing
- Tests use SQLite in-memory database (same pattern as existing tests)
- Coverage: constructor validation, load defaults, load existing, save updates, save inserts, null checks, defaults verification, idempotent load
