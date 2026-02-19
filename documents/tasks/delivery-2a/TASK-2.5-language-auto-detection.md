# Task 2.5: Language Auto-Detection

**Goal:** Enable automatic language detection during transcription and persist the detected language with the recording.
**Part of:** Delivery 2a

## Context
Whisper supports multilingual transcription. By default, it detects the spoken language. VivaVoz needs to leverage this and surface the information to the user.

## Requirements

### Functional
- **Auto-Detect:** Configure Whisper to run in auto-detect mode (default).
- **Metadata:** Capture the detected language code (e.g., `en`, `es`, `pt`) from the transcription result.
- **Persistence:** Store the language code in the `Recording` entity.
- **Display:** Show the detected language in the recordings list (e.g., a small flag or ISO code).
- **Fallback:** If detection is ambiguous or fails, default to "Unknown" or user-configured preferred language.

### Technical
- **Library:** `Whisper.net`. Use `WithLanguage("auto")` (or equivalent API for auto-detection).
- **Entity:** Update `Recording` model with `LanguageCode` (string, ISO 639-1).
- **UI:** Add language column/badge to `RecordingsListViewModel`.

## Acceptance Criteria (Verification Steps)

### Scenario: Detect English
- Record a spoken segment in English.
- Wait for the transcription to complete.
- Verify that the `LanguageCode` for the recording is "en".
- Confirm that the UI displays "English" (or "EN").

### Scenario: Detect Portuguese
- Record a spoken segment in Portuguese.
- Wait for the transcription to complete.
- Verify that the `LanguageCode` for the recording is "pt".
- Confirm that the UI displays "Portuguese" (or "PT").

### Scenario: Store Language Code
- Complete a recording and transcription process.
- Inspect the database entry for the recording.
- Verify that the language detected by Whisper is stored correctly.
- Open the recording details view and confirm the language metadata is visible.
