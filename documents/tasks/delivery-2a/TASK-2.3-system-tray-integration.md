# Task 2.3: System Tray Integration

**Goal:** Allow VivaVoz to run unobtrusively in the background via the system tray.
**Part of:** Delivery 2a

## Context
VivaVoz is a "utility" application. It should be available at a moment's notice but not clutter the taskbar. The system tray is the standard Windows pattern for such tools.

## Requirements

### Functional
- **Tray Icon:** Display an icon in the notification area.
- **Dynamic Icons:** Update the icon based on app state:
    - **Idle:** Standard logo.
    - **Recording:** Red dot/recording indicator.
    - **Transcribing:** Spinner/processing indicator.
    - **Done:** Checkmark (briefly) or back to Idle.
- **Context Menu:** Right-click reveals:
    - **Show VivaVoz:** Open main window.
    - **Start Recording:** Direct command (toggle).
    - **Settings:** Direct link.
    - **Exit:** Fully terminate the application.
- **Window Behavior:**
    - Closing the main window (X) should minimize to tray, not exit (unless configured otherwise).
    - Double-clicking the tray icon toggles window visibility.
- **Startup:** Option to start minimized to tray.

### Technical
- **Class:** `SystemTrayService` (manages `TrayIcon`).
- **Avalonia:** `TrayIcon` component in `App.axaml` or managed dynamically in code-behind.
- **Native Menu:** Use `NativeMenu` for platform-consistent right-click menu.
- **State Binding:** Bind `Icon` property to `ApplicationState` (ViewModel).

## Acceptance Criteria (Verification Steps)

### Scenario: Minimize to Tray
- Open the main application window.
- Click the "Close" (X) button.
- Verify that the window hides instead of terminating the process.
- Confirm the application remains running in the system tray.

### Scenario: Restore from Tray
- Ensure the application is minimized to the tray.
- Double-click the tray icon.
- Verify that the main window appears and takes focus.

### Scenario: Tray Icon Reflects Recording State
- Ensure the application is in an idle state.
- Start a recording.
- Verify that the system tray icon changes to the "Recording" icon.
- Hover over the icon and verify the tooltip displays "Recording...".

### Scenario: Exit Application via Tray
- Ensure the application is running in the background.
- Right-click the tray icon and select "Exit".
- Verify that the application closes completely.
- Check Task Manager to confirm the process has terminated.
