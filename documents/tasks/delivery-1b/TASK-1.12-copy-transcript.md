# Task 1.12: Copy Transcript to Clipboard

**Goal:** Enable users to copy the transcript text to the system clipboard with a single click.
**Part of:** Delivery 1b

## Context
A key workflow is transferring the transcribed thought into another application (e.g., email, Slack, Notion). Manual selection is tedious. A dedicated "Copy" button is standard UX for this type of tool.

## Requirements

### Functional
- A visible "Copy" button should appear near the transcript display.
- Clicking the button copies the entire transcript text to the system clipboard.
- Visual feedback should confirm the action (e.g., button briefly changes to "Copied!", or a toast notification).
- The button should be disabled if no transcript exists (e.g., still recording/transcribing).

### Technical
- **Avalonia:** Use `TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(string text)`. (Note: Avalonia 11+ clipboard access is async and requires a TopLevel reference).
- **ViewModel:** Implement `IAsyncRelayCommand` (or equivalent) for `CopyTranscriptCommand`.
- **View:** `Button` with icon (e.g., `content_copy` from FluentIcons or Material Design).

## Acceptance Criteria (Verification Steps)

### Verify: Copy successful transcript
- [ ] Select a recording with the transcript "Hello world".
- [ ] Click the "Copy" button near the transcript.
- [ ] Paste the clipboard content into a text editor and verify it matches "Hello world".
- [ ] Observe the UI for visual feedback (e.g., button label changes to "Copied!" or a toast appears).

### Verify: Disable copy when processing
- [ ] Select a recording that is currently in `Transcribing` status.
- [ ] Verify that the "Copy" button is disabled (`IsEnabled="False"`) or hidden.

### Verify: Handle empty transcript (silence)
- [ ] Select a recording that is `Complete` but has an empty transcript.
- [ ] Verify that the "Copy" button is disabled.
