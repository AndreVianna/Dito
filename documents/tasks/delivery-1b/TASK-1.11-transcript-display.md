# Task 1.11: Detail View — Transcript Display

**Goal:** Display the transcribed text in the detail panel, allowing users to see the result of their recording.
**Part of:** Delivery 1b

## Context
This is the "Output" phase of the user journey. The user has recorded (Input), the system has transcribed (Process), and now they must view the result (Output). The transcript is the primary artifact.

## Requirements

### Functional
- Display the transcript text prominently below the audio player in the detail view.
- If the recording is still transcribing, show a placeholder or status ("Transcribing...").
- If transcription failed, show an error message ("Transcription failed").
- If the transcript is empty, show "No speech detected" or similar.
- **Note:** For MVP v1.0, direct editing is a separate task (Task 2.8). For this task, display is sufficient (read-only `TextBox` is fine, or editable but no save hook yet - let's stick to read-only for now to keep scope tight). *Wait, PRD F3 says "Edit transcript text manually".* Let's make it an editable `TextBox` bound to the view model, but persistence is Task 2.8. For now, local changes in memory are fine, or just read-only. *Decision: Read-only for this task to keep it small. Editing comes in Task 2.8.*

### Technical
- **View:** `RecordingDetailView.axaml`
- **ViewModel:** `RecordingDetailViewModel`
- **Binding:** `SelectedRecording.Transcript`
- **Control:** `TextBox` (IsReadOnly="True", TextWrapping="Wrap", AcceptsReturn="True")

## Acceptance Criteria (Verification Steps)

### Verify: Display completed transcript
- [ ] Ensure a recording exists with the transcript "The quick brown fox" and status `Complete`.
- [ ] Select this recording in the list.
- [ ] Verify that the detail view displays "The quick brown fox" in the transcript area.
- [ ] Confirm the text is readable and correctly wrapped.

### Verify: Handle ongoing transcription
- [ ] Create a recording with status `Transcribing` and a null/empty transcript.
- [ ] Select this recording in the list.
- [ ] Verify that the detail view displays a "Transcribing..." placeholder or a loading indicator.
- [ ] Ensure the main transcript area is empty or disabled while in this state.

### Verify: Handle failed transcription
- [ ] Create a recording with status `Failed`.
- [ ] Select this recording in the list.
- [ ] Verify that the detail view displays "Transcription failed" or a similar error message.
- [ ] Confirm the error state is visually distinct (e.g., red text or an alert icon).

### Verify: Handle empty transcript (silence)
- [ ] Create a recording with status `Complete` but an empty transcript string.
- [ ] Select this recording in the list.
- [ ] Verify that the detail view displays "No speech detected" or an appropriate placeholder for empty content.
