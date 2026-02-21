# Task 1.11: Detail View — Transcript Display

**Goal:** Display the transcribed text in the detail panel, allowing users to see the result of their recording.
**Part of:** Delivery 1b
**Status:** ✅ Complete

## Context
This is the "Output" phase of the user journey. The user has recorded (Input), the system has transcribed (Process), and now they must view the result (Output). The transcript is the primary artifact.

## Requirements

### Functional
- Display the transcript text prominently below the audio player in the detail view.
- If the recording is still transcribing, show a placeholder or status ("Transcribing...") with a progress indicator.
- If transcription failed, show an error message ("Transcription failed.").
- If the transcript is empty/null with Complete status, show "No speech detected."
- If the recording is still in Recording status, show nothing (empty).
- Read-only display (editing is Task 2.8).

### Technical
- **View:** `RecordingDetailView.axaml`
- **ViewModel:** `MainViewModel` (computed properties)
- **Binding:** `TranscriptDisplay` (computed from `SelectedRecording.Status` + `SelectedRecording.Transcript`)
- **Control:** `TextBox` (IsReadOnly="True", TextWrapping="Wrap", AcceptsReturn="True") — allows text selection for manual copy

## Implementation

### ViewModel properties added to `MainViewModel`:
| Property | Type | Description |
|----------|------|-------------|
| `TranscriptDisplay` | `string` | Computed state-aware transcript text |
| `IsTranscribing` | `bool` | True when selected recording is transcribing (drives progress bar visibility) |
| `IsTranscriptionFailed` | `bool` | True when transcription failed (for future styling) |
| `ShowTranscriptSection` | `bool` | True when any recording is selected |

### State machine for `TranscriptDisplay`:
| Recording Status | Transcript Value | Display Text |
|-----------------|-----------------|--------------|
| `null` (no selection) | — | `""` (empty) |
| `Recording` | — | `""` (empty) |
| `Transcribing` | — | `"Transcribing..."` |
| `Failed` | — | `"Transcription failed."` |
| `Complete` | `null` or `""` | `"No speech detected."` |
| `Complete` | `"Hello world"` | `"Hello world"` |

### View changes (`RecordingDetailView.axaml`):
- Replaced simple `TextBlock` with read-only `TextBox` (supports text selection)
- Added `ProgressBar` (indeterminate) next to "Transcript" header when transcribing
- Transcript section visibility bound to `ShowTranscriptSection`
- TextBox styled to look borderless/transparent (blends with panel)
- Transcript text bound one-way to `TranscriptDisplay`

### PropertyChanged notifications:
- `OnSelectedRecordingChanged` → notifies all 4 transcript properties
- `OnTranscriptionCompleted` → notifies transcript properties when the selected recording completes (live update)

## Acceptance Criteria (Verification Steps)

### ✅ Verify: Display completed transcript
- [x] Recording with transcript "The quick brown fox" and status `Complete` → displays the transcript text
- [x] Text is readable, wrapped, and selectable

### ✅ Verify: Handle ongoing transcription
- [x] Recording with status `Transcribing` → displays "Transcribing..." with progress indicator
- [x] Progress bar is indeterminate (animated)

### ✅ Verify: Handle failed transcription
- [x] Recording with status `Failed` → displays "Transcription failed."

### ✅ Verify: Handle empty transcript (silence)
- [x] Recording with status `Complete` but null/empty transcript → displays "No speech detected."

### ✅ Verify: Live update on transcription completion
- [x] `OnTranscriptionCompleted` calls `NotifyTranscriptProperties()` when selected recording finishes
- [x] Transcript text updates automatically without re-selecting

## Tests Added (22 new tests in `MainViewModelTests.cs`)

### TranscriptDisplay (7 tests)
- `TranscriptDisplay_WhenNoSelection_ShouldBeEmpty`
- `TranscriptDisplay_WhenTranscribing_ShouldShowTranscribingMessage`
- `TranscriptDisplay_WhenFailed_ShouldShowFailedMessage`
- `TranscriptDisplay_WhenCompleteWithNullTranscript_ShouldShowNoSpeechDetected`
- `TranscriptDisplay_WhenCompleteWithEmptyTranscript_ShouldShowNoSpeechDetected`
- `TranscriptDisplay_WhenCompleteWithTranscript_ShouldShowTranscriptText`
- `TranscriptDisplay_WhenRecordingStatus_ShouldBeEmpty`

### IsTranscribing (3 tests)
- `IsTranscribing_WhenNoSelection_ShouldBeFalse`
- `IsTranscribing_WhenTranscribing_ShouldBeTrue`
- `IsTranscribing_WhenComplete_ShouldBeFalse`

### IsTranscriptionFailed (3 tests)
- `IsTranscriptionFailed_WhenNoSelection_ShouldBeFalse`
- `IsTranscriptionFailed_WhenFailed_ShouldBeTrue`
- `IsTranscriptionFailed_WhenComplete_ShouldBeFalse`

### ShowTranscriptSection (2 tests)
- `ShowTranscriptSection_WhenNoSelection_ShouldBeFalse`
- `ShowTranscriptSection_WhenRecordingSelected_ShouldBeTrue`

### PropertyChanged notifications (4 tests)
- `OnSelectedRecordingChanged_ShouldRaiseTranscriptDisplayPropertyChanged`
- `OnSelectedRecordingChanged_ShouldRaiseIsTranscribingPropertyChanged`
- `OnSelectedRecordingChanged_ShouldRaiseIsTranscriptionFailedPropertyChanged`
- `OnSelectedRecordingChanged_ShouldRaiseShowTranscriptSectionPropertyChanged`

### Integration (1 test)
- `TranscriptDisplay_WhenSelectionCleared_ShouldReturnToEmpty` (verifies all properties reset)

## Files Modified
- `source/VivaVoz/ViewModels/MainViewModel.cs` — Added 4 computed properties + `NotifyTranscriptProperties()` method
- `source/VivaVoz/Views/RecordingDetailView.axaml` — Replaced transcript TextBlock with state-aware TextBox + progress indicator
- `source/VivaVoz.Tests/ViewModels/MainViewModelTests.cs` — Added 22 unit tests + `CreateViewModel` helper
