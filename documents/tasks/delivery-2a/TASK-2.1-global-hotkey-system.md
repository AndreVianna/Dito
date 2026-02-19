# Task 2.1: Global Hotkey System

**Goal:** Implement a global hotkey system to control recording from anywhere in Windows.
**Part of:** Delivery 2a

## Context
VivaVoz is designed to be a "headless" recorder. Users shouldn't need to bring the window to the foreground to start recording. This task builds the backend infrastructure for capturing key presses globally and routing them to the recording engine.

## Requirements

### Functional
- **Global Registration:** The application must register a system-wide hotkey (default: `Ctrl+Shift+R`).
- **Modes:** Support two recording modes:
    - **Toggle:** Press once to start, press again to stop.
    - **Push-to-Talk:** Press and hold to record, release to stop.
- **Conflict Detection:** If the hotkey is already taken by another application, the system should return false/error on registration (handled gracefully, logging a warning).
- **State Management:**
    - In **Toggle** mode: Ignore the hotkey if a recording is already starting/stopping to prevent bouncing.
    - In **Push-to-Talk** mode: Trigger 'Start' on key down, 'Stop' on key up.
- **Persistence:** Load the configured hotkey and mode from `Settings` on startup.

### Technical
- **Class:** `GlobalHotkeyService` (implements `IHotkeyService`).
- **Library:** Use standard Win32 APIs via P/Invoke (`user32.dll`: `RegisterHotKey`, `UnregisterHotKey`) or a lightweight wrapper like `SharpHook` if complex combos are needed. *Decision: Stick to P/Invoke for native Windows reliability unless cross-platform requirement overrides (MVP is Windows-first).*
- **Event Aggregation:** Publish `RecordingRequestedEvent` and `RecordingStoppedEvent` via the internal message bus/mediator when hotkeys trigger.

## Acceptance Criteria (Verification Steps)

### Scenario: Toggle Mode - Start Recording
- Ensure the application is running in the background.
- Set the recording mode to "Toggle" and the hotkey to "Ctrl+Shift+R".
- Press "Ctrl+Shift+R".
- Verify that the application starts recording.
- Verify that the system tray icon changes to the "Recording" state.

### Scenario: Toggle Mode - Stop Recording
- Start a recording session in "Toggle" mode.
- Press "Ctrl+Shift+R" again.
- Verify that the application stops recording.
- Verify that the recording file is saved correctly.

### Scenario: Push-to-Talk Mode
- Set the recording mode to "Push-to-Talk" and the hotkey to "Ctrl+Shift+R".
- Press and hold "Ctrl+Shift+R".
- Verify that the application starts recording while the key is held.
- Release "Ctrl+Shift+R".
- Verify that the application stops recording immediately.

### Scenario: Hotkey Conflict
- Ensure another application has already registered "Ctrl+Shift+R" globally.
- Launch VivaVoz.
- Verify that the registration attempt fails gracefully.
- Check logs for a warning entry.
- Verify that the user receives a notification (toast or initial UI warning) about the conflict.
