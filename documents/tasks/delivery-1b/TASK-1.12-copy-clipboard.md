# Task 1.12: Copy Transcript to Clipboard

**Goal:** Enable users to copy the transcript text to the system clipboard with a single click.
**Part of:** Delivery 1b
**Status:** ✅ Complete

## Context
A key workflow is transferring the transcribed thought into another application (e.g., email, Slack, Notion). Manual selection is tedious. A dedicated "Copy" button is standard UX for this type of tool.

## Requirements

### Functional
- A visible "Copy" button should appear near the transcript display.
- Clicking the button copies the entire transcript text to the system clipboard.
- Visual feedback should confirm the action (button briefly changes to "Copied!" for 2 seconds).
- The button should be hidden if no copyable transcript exists (transcribing, failed, empty, or no selection).

### Technical
- **Clipboard abstraction:** `IClipboardService` interface for testability, with `ClipboardService` implementation using Avalonia's `TopLevel.GetTopLevel(mainWindow).Clipboard`.
- **ViewModel:** `CopyTranscriptCommand` (async relay command) on `MainViewModel`. Guard property `CanCopyTranscript` checks for `Complete` status with non-empty transcript.
- **Visual feedback:** `CopyButtonLabel` observable property toggles "Copy" → "Copied!" → "Copy" (2s delay).
- **View:** Button in transcript header bar, visibility bound to `CanCopyTranscript`.

## Implementation Details

### Files Created
- `source/VivaVoz/Services/IClipboardService.cs` — Clipboard abstraction interface
- `source/VivaVoz/Services/ClipboardService.cs` — Avalonia clipboard implementation (ExcludeFromCodeCoverage)

### Files Modified
- `source/VivaVoz/ViewModels/MainViewModel.cs` — Added `IClipboardService` dependency, `CopyTranscriptCommand`, `CopyButtonLabel`, `CanCopyTranscript`
- `source/VivaVoz/Views/RecordingDetailView.axaml` — Added Copy button in transcript header
- `source/VivaVoz/App.axaml.cs` — Wired `ClipboardService` into `MainViewModel`
- `source/VivaVoz.Tests/ViewModels/MainViewModelTests.cs` — Added 12 new tests for clipboard functionality

### New Tests (12)
1. `Constructor_WithNullClipboardService_ShouldThrowArgumentNullException`
2. `CanCopyTranscript_WhenNoSelection_ShouldBeFalse`
3. `CanCopyTranscript_WhenTranscribing_ShouldBeFalse`
4. `CanCopyTranscript_WhenFailed_ShouldBeFalse`
5. `CanCopyTranscript_WhenCompleteWithNullTranscript_ShouldBeFalse`
6. `CanCopyTranscript_WhenCompleteWithEmptyTranscript_ShouldBeFalse`
7. `CanCopyTranscript_WhenCompleteWithTranscript_ShouldBeTrue`
8. `OnSelectedRecordingChanged_ShouldRaiseCanCopyTranscriptPropertyChanged`
9. `CopyTranscriptCommand_WhenCompleteWithTranscript_ShouldCopyTextToClipboard`
10. `CopyTranscriptCommand_WhenNoTranscript_ShouldNotCallClipboard`
11. `CopyTranscriptCommand_WhenTranscribing_ShouldNotCallClipboard`
12. `CopyTranscriptCommand_WhenNoSelection_ShouldNotCallClipboard`
13. `CopyTranscriptCommand_WhenExecuted_ShouldChangeLabelToCopied`
14. `CopyButtonLabel_WhenNewInstance_ShouldBeCopy`
15. `CopyButtonLabel_WhenSelectionChanges_ShouldResetToCopy`

### Total test count: 197 (all passing)

## Acceptance Criteria (Verification Steps)

### Verify: Copy successful transcript
- [x] Select a recording with the transcript "Hello world".
- [x] Click the "Copy" button near the transcript.
- [x] Paste the clipboard content into a text editor and verify it matches "Hello world".
- [x] Observe the UI for visual feedback — button label changes to "Copied!" for 2 seconds.

### Verify: Hide copy when not applicable
- [x] Select a recording that is currently in `Transcribing` status → Copy button is hidden.
- [x] Select a recording that is `Complete` but has an empty transcript → Copy button is hidden.
- [x] No recording selected → Copy button is hidden.

## Dependencies
- Task 1.11 (Transcript Display) — uses `TranscriptDisplay`, `ShowTranscriptSection`, and transcript-related properties
