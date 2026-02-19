# Task 2.11: Theme Support

**Goal:** Allow users to choose between Light, Dark, or System theme preferences.
**Part of:** Delivery 2b

## Context
Visual comfort and integration with the OS are key for a native app. VivaVoz should respect the user's system preference by default but allow manual override.

## Requirements

### Functional
- **Settings Option:** "Theme" dropdown in Settings (System, Light, Dark).
- **Default:** "System" (follow Windows setting).
- **Immediate Effect:** Changing the setting updates the UI instantly without restart.
- **Persistence:** Remember the choice across sessions.
- **Fluent Design:** Use Avalonia's built-in Fluent theme styles.

### Technical
- **Avalonia:** Use `Application.Current.RequestedThemeVariant`.
- **Entity:** Update `Settings` model with `Theme` enum (System, Light, Dark).
- **Service:** `IThemeService` or within `ISettingsService`.

## Acceptance Criteria (Verification Steps)

- **Verify Switch to Dark Mode:**
  - Start the application in Light mode.
  - Navigate to Settings.
  - Select "Dark" from the Theme dropdown.
  - Verify that the application UI immediately switches to Dark colors.
  - Verify that the setting "Dark" is saved in the configuration.

- **Verify Switch to Light Mode:**
  - Start the application in Dark mode.
  - Navigate to Settings.
  - Select "Light" from the Theme dropdown.
  - Verify that the application UI immediately switches to Light colors.
  - Verify that the setting "Light" is saved in the configuration.

- **Verify Switch to System Default:**
  - Set the OS (Windows) theme to Dark mode.
  - Start the application in Light mode (manual override).
  - Navigate to Settings.
  - Select "System" from the Theme dropdown.
  - Verify that the application UI immediately switches to Dark colors (matching the OS).
  - Verify that the setting "System" is saved.

- **Verify Theme Persistence:**
  - Set the theme to "Dark" (or a non-default value).
  - Close the application.
  - Relaunch the application.
  - Verify that the UI loads in Dark mode without requiring manual intervention.
