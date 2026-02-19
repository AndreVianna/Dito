# Task 2.6: Settings Screen

**Goal:** Provide a central UI for configuring all application preferences.
**Part of:** Delivery 2a

## Context
Users need to tailor VivaVoz to their workflow (hotkeys, models) and environment (dark mode, storage location). This screen consolidates all configuration.

## Requirements

### Functional
- **General:**
    - **Run at Startup:** Toggle (Windows Registry/Task Scheduler).
    - **Minimize to Tray on Close:** Toggle.
    - **Theme:** System/Light/Dark (immediate application).
- **Recording:**
    - **Input Device:** Dropdown list of available microphones (NAudio).
    - **Hotkey:** Key recorder (capture user input: e.g., press keys to set `Ctrl+Alt+R`).
    - **Mode:** Radio buttons: Toggle / Push-to-Talk.
- **Transcription:**
    - **Model:** Dropdown (Tiny, Base, etc.) + "Manage Models" button (opens Task 2.4 view).
    - **Language:** Auto / Fixed (e.g., force English).
- **Storage:**
    - **Location:** Text box + "Browse..." button to set root folder.
    - **Export Format:** Dropdown (MP3, WAV, OGG, TXT, MD).

### Technical
- **View:** `SettingsView` (Avalonia UserControl/Window).
- **ViewModel:** `SettingsViewModel`.
- **Persistence:** Bi-directional binding to `SettingsService` (SQLite/JSON). Changes save immediately or on "Apply". *Decision: Immediate save for UX simplicity, or "Save" button if validation complex.* Let's go with immediate save + validation.

## Acceptance Criteria (Verification Steps)

### Scenario: Change Input Device
- Ensure multiple microphones are connected to the system.
- Select "Microphone (USB)" (or a specific alternative device) from the input device dropdown in Settings.
- Perform a test recording.
- Verify that the application uses the selected device.
- Restart the application.
- Verify that the selected input device persists.

### Scenario: Configure Hotkey
- Navigate to the Settings screen.
- Focus on the hotkey input field and press a key combination (e.g., "Ctrl+Alt+R").
- Verify that the hotkey setting updates to display the new combination.
- Confirm that the old hotkey is unregistered and no longer triggers a recording.
- Test the new hotkey and verify it is active immediately.

### Scenario: Toggle Theme
- Ensure the current theme is set to "Light".
- Select "Dark" from the theme dropdown.
- Verify that the application UI switches to dark colors immediately.
- Restart the application.
- Verify that the "Dark" theme setting is preserved.

### Scenario: Change Storage Path
- Note the default storage path.
- Change the storage path to a new directory (e.g., "D:\MyRecordings").
- Perform a test recording.
- Verify that the new recording is saved in the new location.
- (MVP Note: Existing recordings do not need to be migrated, only new ones).
