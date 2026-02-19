# Task 2.15: Crash Recovery

**Goal:** Implement a safety mechanism to prevent data loss in case of an unexpected application crash or power failure during recording.
**Part of:** Delivery 2b

## Context
A voice recorder is a "capture" tool. If the capture is lost because the app crashed, the user's trust is lost forever. We must guarantee that if a recording starts, it is recoverable.

## Requirements

### Functional
- **Real-time Buffer:** While recording, audio data must be continuously flushed to a temporary file on disk (not just kept in memory).
- **Orphan Detection:** On application startup, check for any `.tmp` or incomplete audio files in the temp directory.
- **Recovery Prompt:** If an orphan is found, display a modal dialog: "VivaVoz recovered an interrupted recording. Would you like to save it?"
- **Action - Keep:** Move the file to the permanent `audio/` folder, create a database entry (Title: "Recovered Recording"), and transcribe it.
- **Action - Discard:** Delete the temporary file.

### Technical
- **Service:** `ICrashRecoveryService`.
  - `CheckForRecoverableRecordings()`
  - `RecoverAsync(string tempFilePath)`
  - `DiscardAsync(string tempFilePath)`
- **Path:** `%LOCALAPPDATA%/VivaVoz/temp/recording.buffer` (or similar).
- **Logic:** Ensure `AudioRecorder` writes to disk periodically (e.g., every 500ms or 1s buffer flush).

## Acceptance Criteria (Verification Steps)

- **Verify Recover Recording after Crash:**
  - Start a recording.
  - While recording is active, force kill the application process (e.g., Task Manager or `kill` command).
  - Relaunch the application.
  - Verify that a "Crash Recovery" dialog appears.
  - Verify that the dialog message states "We found an unfinished recording" (or similar).
  - Verify that the dialog offers "Save" and "Discard" buttons.

- **Verify Save Recovered Recording:**
  - Trigger the "Crash Recovery" dialog as described above.
  - Click "Save".
  - Verify that the recovered audio file appears in the "Recordings" list.
  - Verify that the title is automatically set to "Recovered Recording [Date/Time]".
  - Verify that transcription starts automatically for the recovered file.

- **Verify Discard Recovered Recording:**
  - Trigger the "Crash Recovery" dialog.
  - Click "Discard".
  - Verify that the temporary file is permanently deleted from disk.
  - Verify that the application proceeds to the main window as normal.
  - Verify that no recovered recording appears in the list.
