# Task 2.8: Recording CRUD — Edit + Delete

**Goal:** Provide full control over the recording lifecycle, allowing users to correct transcripts and remove unwanted files.
**Part of:** Delivery 2a

## Context
Transcripts are rarely perfect. Users need to fix typos, names, or misheard words. They also need to delete test recordings or sensitive data.

## Requirements

### Functional
- **Edit:**
    - Enable editing of the transcript text area.
    - "Edit" button toggles read-only state.
    - "Save" commits changes to DB.
    - "Cancel" reverts to original.
- **Delete:**
    - "Delete" button (trash icon).
    - Confirmation dialog: "Are you sure you want to delete this recording? This cannot be undone."
    - Removes:
        - Database record.
        - Audio file (`.wav`).
        - Exported files (if tracked/in same folder - MVP scope: just .wav).
- **Undo:** Nice to have, but confirm dialog covers safety for MVP.

### Technical
- **ViewModel:** `RecordingDetailViewModel` (Edit mode state).
- **Service:** `IRecordingService.UpdateAsync(recording)`, `IRecordingService.DeleteAsync(id)`.
- **File System:** Ensure `File.Delete(path)` is called safely (check existence, try/catch for locks).

## Acceptance Criteria (Verification Steps)

### Scenario: Edit Transcript
- View a recording with a known transcript (e.g., "Hello worl").
- Click the "Edit" button.
- Change the text to "Hello World".
- Click "Save".
- Verify that the transcript is updated in the database.
- Confirm that the view reflects the updated text "Hello World".
- Verify that the "Edit" mode closes.

### Scenario: Cancel Edit
- Start editing a transcript and make changes.
- Click "Cancel".
- Verify that the changes are discarded.
- Confirm that the view shows the original transcript.

### Scenario: Delete Recording - Confirm
- Select a recording.
- Click the "Delete" button.
- Verify that a confirmation dialog appears.
- Confirm "Yes".
- Verify that the recording is removed from the list.
- Confirm that the corresponding audio file is deleted from the disk.

### Scenario: Delete Recording - Cancel
- Select a recording.
- Click the "Delete" button.
- Cancel the confirmation dialog.
- Verify that the recording remains in the list.
- Confirm that no files are deleted.
