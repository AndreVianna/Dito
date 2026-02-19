# Task 2.4: Model Management

**Goal:** Allow users to download, manage, and switch between different Whisper model sizes to balance speed vs. accuracy.
**Part of:** Delivery 2a

## Context
Whisper models vary significantly in size and capability. The Tiny model is fast but less accurate. Large models are slow but highly precise. Users need control over this trade-off based on their hardware.

## Requirements

### Functional
- **Default:** Installer includes `ggml-tiny.bin` (or downloads on first run if size is prohibitive, but PRD says bundled).
- **Listing:** Show available models: Tiny, Base, Small, Medium, Large.
- **Status:** Indicate "Installed", "Not Installed", or "Downloading...".
- **Download:** Button to fetch models. Must show:
    - **Progress Bar:** Percentage/Speed.
    - **Cancel:** Abort download.
    - **Size:** Expected disk usage.
- **Selection:** Choose active model for transcription.
- **Management:** Delete installed models to free space.
- **Path:** Store in `%LOCALAPPDATA%/VivaVoz/models/`.

### Technical
- **Service:** `WhisperModelService` (implements `IModelManager`).
- **Source:** Download `.bin` files from a reliable mirror (e.g., huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-{model}.bin).
- **Concurrency:** Async/await for downloads; do not block UI.
- **Validation:** Check file hash (SHA256) after download to ensure integrity.

## Acceptance Criteria (Verification Steps)

### Scenario: List Available Models
- Launch the application and navigate to the "Models" settings page.
- Verify that all supported Whisper models (Tiny, Base, Small, Medium, Large) are listed.
- Check that each model displays its current installation status (Installed/Not Installed).

### Scenario: Download a Model
- Ensure the "Small" model is not installed.
- Click the "Download" button for the "Small" model.
- Verify that a download progress bar appears and the status changes to "Downloading...".
- Wait for the download to complete.
- Verify that the status updates to "Installed".
- Confirm that the model file exists in the models directory.

### Scenario: Delete a Model
- Ensure the "Medium" model is installed.
- Click the "Delete" button for the "Medium" model.
- Verify that the model file is removed from the disk.
- Confirm that the status reverts to "Not Installed".
- If "Medium" was the active model, verify that the active model setting reverts to "Tiny" (or another available default).

### Scenario: Select Active Model
- Ensure the "Base" model is installed.
- Select "Base" as the active model.
- Perform a test transcription.
- Verify that the transcription uses the "Base" model file.
- Restart the application and confirm the "Base" model selection persists.
