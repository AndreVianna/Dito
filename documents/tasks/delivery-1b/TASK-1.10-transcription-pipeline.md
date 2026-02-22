# Task 1.10: Transcription Pipeline

**Goal:** Automate the transcription process immediately after a recording is stopped.
**Part of:** Delivery 1b
**Status:** ✅ Complete

## Context
The core flow of VivaVoz is "Click → Speak → Click → Text". Task 1.9 gave us the ability to transcribe. This task automates it. When the user stops recording, the application must immediately begin processing the audio in the background and update the recording status.

## Requirements

### Functional
- When a recording stops, the `Recording` status must transition from `Recording` to `Transcribing`.
- The transcription service (Task 1.9) must be invoked asynchronously.
- Upon successful transcription:
    - `Recording.Transcript` is updated with the text.
    - `Recording.Status` becomes `Complete`.
    - `Recording.Language` and `Recording.WhisperModel` are updated from the transcription result.
    - `Recording.UpdatedAt` is set to `DateTime.UtcNow`.
- Upon failure:
    - `Recording.Status` becomes `Failed`.
    - Error details are logged (Task 1.4).
    - `Recording.Transcript` remains null.

### Technical
- **TranscriptionManager:** New service that orchestrates background transcription without blocking the UI.
- **Entity Update:** Recording status and transcript are persisted to SQLite via a separate `AppDbContext` instance (thread-safe via factory pattern).
- **Status Enum:** `RecordingStatus` enum already exists with `Recording`, `Transcribing`, `Complete`, `Failed`.

## Implementation

### Architecture
```
AudioRecorderService.RecordingStopped
  → MainViewModel.OnRecordingStopped (UI thread)
    → Creates Recording with Status=Transcribing, saves to DB
    → Calls TranscriptionManager.EnqueueTranscription(recordingId, filePath)
      → Task.Run → ProcessTranscriptionAsync (thread pool)
        → ITranscriptionEngine.TranscribeAsync
        → Updates DB (success: Complete + transcript; failure: Failed)
        → Fires TranscriptionCompleted event
  → MainViewModel.OnTranscriptionCompleted (UI thread via Dispatcher)
    → Updates in-memory Recording object for UI binding
```

### Files Created
| File | Description |
|------|-------------|
| `Services/Transcription/ITranscriptionManager.cs` | Interface: `EnqueueTranscription` + `TranscriptionCompleted` event |
| `Services/Transcription/TranscriptionManager.cs` | Background transcription orchestrator using `ITranscriptionEngine` |
| `Services/Transcription/TranscriptionCompletedEventArgs.cs` | Event args with static factories `Succeeded` and `Failed` |
| `Tests/Services/Transcription/TranscriptionManagerTests.cs` | 14 tests covering success, failure, DB updates, events, edge cases |
| `Tests/Services/Transcription/TranscriptionCompletedEventArgsTests.cs` | 12 tests covering both factory methods |

### Files Modified
| File | Change |
|------|--------|
| `ViewModels/MainViewModel.cs` | Added `ITranscriptionManager` dependency, triggers `EnqueueTranscription` after recording stops, handles `TranscriptionCompleted` to update UI-bound objects |
| `App.axaml.cs` | Creates `WhisperModelManager`, `WhisperTranscriptionEngine`, `TranscriptionManager` and passes to MainViewModel |
| `Tests/ViewModels/MainViewModelTests.cs` | Updated all 29+ tests to pass mock `ITranscriptionManager`, added null guard test |

### Key Design Decisions
1. **Func\<AppDbContext\> factory pattern** — EF Core DbContext is not thread-safe. The `TranscriptionManager` creates a separate context for each background operation via a factory, ensuring thread safety.
2. **Fire-and-forget via Task.Run** — `EnqueueTranscription` is synchronous (returns void) and immediately offloads work to the thread pool, ensuring zero UI thread blocking.
3. **Event-based UI notification** — `TranscriptionCompleted` event allows the ViewModel to update in-memory objects on the UI thread via `Dispatcher.UIThread.Post`.
4. **CancellationTokenSource with Dispose** — The manager supports graceful shutdown. Disposed flag prevents double-dispose.
5. **Failure isolation** — If the DB update for a failure status itself fails, it's logged but doesn't propagate (double-fault protection).

## Acceptance Criteria (Verification Steps)

### Verify: Successful automatic transcription
- [x] Recording status transitions to `Transcribing` immediately on stop.
- [x] `ITranscriptionEngine.TranscribeAsync` is invoked with the correct audio file path.
- [x] On success, recording status updates to `Complete` in the database.
- [x] Transcript text, detected language, and model are stored in the database.
- [x] `UpdatedAt` timestamp is refreshed.
- [x] `TranscriptionCompleted` event fires with success data.

### Verify: Transcription failure handling
- [x] On engine failure, recording status updates to `Failed` in the database.
- [x] Transcript remains null on failure.
- [x] `TranscriptionCompleted` event fires with error message.
- [x] `UpdatedAt` timestamp is refreshed even on failure.
- [x] Error is logged via Serilog.

### Verify: Edge cases
- [x] Non-existent recording ID (DB deleted between enqueue and process) — handled gracefully, event still fires.
- [x] Null/empty/whitespace file path — throws `ArgumentException` synchronously.
- [x] Double dispose — safe, no exceptions.
- [x] Null constructor dependencies — throws `ArgumentNullException`.

## Test Summary
- **TranscriptionManagerTests:** 14 tests — success pipeline, failure pipeline, DB updates, event data, edge cases
- **TranscriptionCompletedEventArgsTests:** 12 tests — both factory methods, all properties
- **MainViewModelTests:** 1 new test (null transcription manager guard), 29 updated tests
- **Total new/modified tests:** 27 new + 29 updated = 56 tests touched
- **All 162 tests pass.**
