# Task 2.14: Graceful Degradation

**Goal:** Ensure the app continues to function even when preferred resources (models, devices, locations) fail.
**Part of:** Delivery 2b

## Context
A perfect app works perfectly in perfect conditions. VivaVoz must work in imperfect ones. If a large model is corrupt, use the tiny one. If a specific mic is unplugged, use the default. If saving fails, copy to clipboard. Always offer a Plan B.

## Requirements

### Functional
- **Model Fallback:** If the selected Whisper model (e.g., Large) fails to load or is missing, automatically fall back to the bundled "Tiny" model. Notify the user with a warning toast: "Using Tiny model (Large model unavailable)."
- **Audio Device Fallback:** If the configured input device is disconnected, automatically switch to the System Default Input Device. Notify user.
- **Export Fallback:** If saving a file fails (e.g., permission denied), offer to "Copy Transcript to Clipboard" as an immediate alternative action in the error dialog.

### Technical
- **Service Logic:**
  - `TranscriptionService.TranscribeAsync`: Catch model load exceptions → retry with `Tiny`.
  - `AudioRecorder.Start`: Check device presence → fallback to `WaveInEvent.GetDefaultDevice()`.
  - `ExportService.Export`: Catch IO exceptions → return failure result with `CanCopyToClipboard = true`.

## Acceptance Criteria (Verification Steps)

- **Verify Model Fallback:**
  - Configure the application to use a non-default Whisper model (e.g., "Large").
  - Simulate a failure by renaming, deleting, or corrupting the Large model file.
  - Start a transcription.
  - Verify that the system automatically falls back to the "Tiny" model.
  - Verify that a warning toast appears with the message "Large model unavailable. Using Tiny." (or similar).
  - Verify that the transcription completes successfully despite the missing model.

- **Verify Audio Device Fallback:**
  - Select a specific audio input device (e.g., "Blue Yeti Microphone") in Settings.
  - Disconnect the device or simulate its unavailability.
  - Press the Record hotkey.
  - Verify that the system automatically switches to the "Default Input Device".
  - Verify that a warning toast appears with the message "Blue Yeti not found. Using default microphone." (or similar).
  - Verify that recording starts immediately without requiring user intervention.

- **Verify Export Fallback Option:**
  - Attempt to export a transcript to a location that triggers an error (e.g., read-only folder, invalid path).
  - Verify that the export fails and a "Recoverable" error dialog appears with the message "Could not save file."
  - Verify that the dialog offers a "Copy to Clipboard instead" button.
  - Click the button.
  - Paste into a text editor to verify that the transcript text was successfully copied to the clipboard.
