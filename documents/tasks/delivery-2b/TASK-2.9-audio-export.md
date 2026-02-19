# Task 2.9: Audio Export

**Goal:** Allow users to export recordings as audio files (MP3, WAV, OGG) via a standard Save dialog.
**Part of:** Delivery 2b

## Context
Users record voice notes and need to share them or use them in other applications. While VivaVoz stores raw audio internally, users need a way to get it out in common formats. This task bridges the internal `audio/` storage to the user's file system.

## Requirements

### Functional
- **Export Button:** A button in the detail view to export audio.
- **Format Selection:** Support MP3 (default), WAV, and OGG.
- **Save Dialog:** Use the native OS "Save File" dialog to let the user choose the destination and filename.
- **Default Filename:** Suggest a filename based on the recording title or date (e.g., `VivaVoz_2024-03-20_MyNote.mp3`).
- **Conversion:** Convert the internal WAV source to the selected format on demand (if not already cached/available).
- **Error Handling:** If export fails (disk full, permission denied), show a user-friendly error.

### Technical
- **Library:** `NAudio` (or `NAudio.Lame` / `NAudio.Vorbis` if needed for MP3/OGG encoding) or `FFmpeg.AutoGen` if a lighter wrapper is preferred. *Decision from PRD: NAudio.*
- **Interface:** `IAudioExporter` service.
  - `ExportAsync(string sourcePath, string destinationPath, AudioFormat format)`
- **UI:** `SaveFileDialog` from Avalonia.
- **Persistence:** Remember the last used export format in `Settings`.

## Acceptance Criteria (Verification Steps)

- **Verify Export as MP3:**
  - Create a recording titled "Meeting Notes".
  - Ensure the internal audio file exists (e.g., `audio/2026-02/guid.wav`).
  - Click the "Export Audio" button in the detail view.
  - Select "MP3" as the format in the Save Dialog.
  - Choose a destination (e.g., `C:/Users/Andre/Desktop/Notes.mp3`).
  - Confirm the action.
  - Verify that the system converts the audio to MP3 and saves it to the chosen path.
  - Verify that a success notification "Audio exported successfully" is displayed.

- **Verify Export as WAV:**
  - Create a recording titled "Idea 1".
  - Click "Export Audio".
  - Select "WAV" as the format in the Save Dialog.
  - Choose a valid destination path.
  - Confirm the action.
  - Verify that the system copies the internal WAV file to the destination.
  - Verify that a success notification is displayed.

- **Verify Export Cancellation:**
  - Click "Export Audio".
  - Wait for the file dialog to open.
  - Click "Cancel".
  - Verify that no file is created at any destination.
  - Verify that no error message or notification is shown.

- **Verify Export Failure (Disk Full):**
  - Attempt to export a recording to a destination with insufficient space (mock or simulate full disk).
  - Confirm the save location.
  - Verify that the system displays a "Recoverable" error dialog with the message "Export failed. Disk full." (or similar).
  - Verify that the file is not saved (or is cleaned up if partially written).
