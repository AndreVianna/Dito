# Task 1.9: Whisper Integration

**Goal:** Integrate the Whisper.net library and the 'tiny' model into the solution to enable local speech-to-text.
**Part of:** Delivery 1b

## Context
This task lays the foundation for the core value proposition of VivaVoz: local, offline transcription. We are using `Whisper.net` as a .NET wrapper around `whisper.cpp` for high-performance inference. This task focuses purely on the *capability* to transcribe, not the UI or automation pipeline (which comes in Task 1.10).

## Requirements

### Functional
- The application must be able to load a Whisper model file from disk.
- The application must be able to transcribe a WAV audio file into text using the loaded model.
- The 'tiny' model (`ggml-tiny.bin`) must be bundled with the application so it works out-of-the-box without internet.

### Technical
- **NuGet Packages:**
    - `Whisper.net`
    - `Whisper.net.Runtime.Clblast` (for GPU acceleration if possible, otherwise CPU default `Whisper.net.Runtime`) -> *Decision: Start with CPU default `Whisper.net.Runtime` for MVP simplicity and broad compatibility.*
- **File Placement:**
    - Place `ggml-tiny.bin` in `Create/Assets/Models/` (or similar) and configure it to copy to output directory.
    - On startup, ensure the model is copied to `%LOCALAPPDATA%/VivaVoz/models/whisper-tiny.bin` if not present.
- **Service Interface:**
    - Create `IWhisperService` and `WhisperService`.
    - Method: `Task<string> TranscribeAsync(string audioFilePath, string modelPath, string language = "auto");`

## Acceptance Criteria (Verification Steps)

### Verify: Bundled model loading
- [ ] Ensure the application includes `whisper-tiny.bin` in the output directory after build.
- [ ] Initialize `WhisperService` with the path to `whisper-tiny.bin`.
- [ ] Verify that `WhisperService` initializes successfully without throwing errors.
- [ ] Confirm the model is loaded into memory (e.g. by checking logs or debug output).

### Verify: Transcribe a valid WAV file
- [ ] Prepare a valid WAV file containing clear English speech (e.g., "Hello world").
- [ ] Call `TranscribeAsync` with the path to this WAV file.
- [ ] Verify that the returned string contains "Hello world" (case-insensitive).
- [ ] Confirm the operation completes within a reasonable time (e.g., < 5 seconds for a short clip).

### Verify: Handle missing audio file
- [ ] Call `TranscribeAsync` with a file path that does not exist (e.g., `ghost.wav`).
- [ ] Verify that the service throws a `FileNotFoundException`.
- [ ] Check application logs to ensure the error was captured.

### Verify: Handle unsupported audio format
- [ ] Create a dummy text file named `not_audio.txt`.
- [ ] Call `TranscribeAsync` with the path `not_audio.txt`.
- [ ] Verify that the service throws an appropriate exception or returns an error indicating invalid format.
