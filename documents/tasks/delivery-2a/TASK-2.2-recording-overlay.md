# Task 2.2: Recording Overlay

**Goal:** Create a minimal, always-on-top overlay window to indicate recording status and provide a quick stop control.
**Part of:** Delivery 2a

## Context
When recording via global hotkey, the user needs visual confirmation that the app is listening. A subtle, floating overlay provides this feedback without stealing focus or cluttering the screen.

## Requirements

### Functional
- **Visibility:** Show overlay immediately when recording starts; hide immediately when stopped.
- **Position:** Default to bottom-center or last remembered position.
- **Always On Top:** Must float above other windows.
- **Content:**
    - Recording indicator (red dot/icon).
    - Duration timer (MM:SS).
    - Simple waveform visualization (can be a mock animation for MVP, or real-time if feasible).
- **Interactivity:**
    - **Draggable:** User can move it anywhere.
    - **Click-to-Stop:** Clicking the main area or a specific "Stop" button halts recording.
- **Focus:** Must not steal keyboard focus when appearing (should not interrupt typing).

### Technical
- **Class:** `RecordingOverlayWindow` (Avalonia `Window`).
- **Properties:**
    - `SystemDecorations="None"` (frameless).
    - `TransparencyLevelHint="Transparent"` (rounded corners/shaped window).
    - `Topmost="True"`.
    - `ShowInTaskbar="False"`.
- **ViewModel:** `RecordingOverlayViewModel` (binds to `RecordingService` state/duration).

## Acceptance Criteria (Verification Steps)

### Scenario: Overlay Appears on Record
- Start a recording using the global hotkey.
- Verify that the overlay window appears immediately.
- Confirm the timer starts counting from 00:00.
- Verify that the overlay is positioned topmost above other windows.

### Scenario: Overlay Does Not Steal Focus
- Open a text editor and begin typing.
- Press the global record hotkey while typing.
- Verify that the overlay appears without interrupting the text input.
- Confirm the text editor remains the active window.

### Scenario: Click to Stop
- Ensure the overlay is visible and a recording is active.
- Click the "Stop" button on the overlay.
- Verify that the recording stops.
- Verify that the overlay disappears immediately.

### Scenario: Dragging Overlay
- Ensure the overlay is visible.
- Click and drag the overlay to a new screen position.
- Verify the overlay moves smoothly with the cursor.
- Restart the application or start a new recording session.
- Verify the overlay appears at the last dragged position.
