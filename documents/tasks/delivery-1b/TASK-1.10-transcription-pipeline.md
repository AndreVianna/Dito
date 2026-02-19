# Task 1.10: Transcription Pipeline

**Goal:** Automate the transcription process immediately after a recording is stopped.
**Part of:** Delivery 1b

## Context
The core flow of VivaVoz is "Click -> Speak -> Click -> Text". Task 1.9 gave us the ability to transcribe. This task automates it. When the user stops recording, the application must immediately begin processing the audio in the background and update the recording status.

## Requirements

### Functional
- When a recording stops, the `Recording` status must transition from `Recording` to `Transcribing`.
- The transcription service (Task 1.9) must be invoked asynchronously.
- Upon successful transcription:
    - `Recording.Transcript` is updated with the text.
    - `Recording.Status` becomes `Complete`.
    - `Recording.UpdatedAt` is set to `DateTime.UtcNow`.
- Upon failure:
    - `Recording.Status` becomes `Failed`.
    - Error details are logged (Task 1.4).
    - `Recording.Transcript` remains null/empty (or contains error message, decision: null).

### Technical
- **RecordingService:** Extend `StopRecordingAsync` to call `_whisperService.TranscribeAsync` without blocking the UI thread.
- **Entity Update:** Ensure `Recording.Status` and `Transcript` properties are persisted to SQLite via `IRepository<Recording>` (or equivalent).
- **Status Enum:** Verify `RecordingStatus` enum exists (`Recording`, `Transcribing`, `Complete`, `Failed`).

## Acceptance Criteria (Verification Steps)

### Verify: Successful automatic transcription
- [ ] Start a recording and speak "Testing one two three".
- [ ] Stop the recording.
- [ ] Verify that the recording status updates to `Transcribing` almost immediately (< 200ms).
- [ ] Confirm that `WhisperService` is invoked with the correct audio file path.
- [ ] Wait for transcription to complete.
- [ ] Verify that the recording status updates to `Complete`.
- [ ] Check the database or UI to confirm the transcript contains "Testing one two three".

### Verify: Transcription failure handling
- [ ] Mock the `WhisperService` or file system to simulate a failure (e.g., lock the file or throw an exception).
- [ ] Start and stop a recording.
- [ ] Verify that the status transitions to `Transcribing`.
- [ ] Confirm that after the failure, the status updates to `Failed`.
- [ ] Check `vivavoz.log` to ensure the error was logged with details.

### Verify: Zero-length recording (too short)
- [ ] Start a recording and stop it immediately (< 0.5 seconds).
- [ ] Verify that the application handles the empty/invalid audio file gracefully.
- [ ] Confirm the recording status ends up as `Failed` or `Complete` (with empty transcript), depending on implemented strategy.
- [ ] Ensure the application does not crash.
