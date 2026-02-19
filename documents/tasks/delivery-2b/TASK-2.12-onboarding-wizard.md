# Task 2.12: First-Run Onboarding Wizard

**Goal:** Create a 4-step wizard to guide new users through initial setup and first success.
**Part of:** Delivery 2b

## Context
First impressions matter. VivaVoz needs to prove value immediately. Instead of dropping users into an empty list, we guide them to pick a model, set a hotkey, and record a test note. This ensures the app is configured correctly and the user understands the core loop.

## Requirements

### Functional
- **Trigger:** Only show on first launch (or if `HasCompletedOnboarding` is false).
- **Step 1: Welcome:** Value proposition ("Your voice, alive"). "Get Started" button.
- **Step 2: Model Selection:** Choose Tiny (fastest, bundled) or Base (better). If Base, start download. Show progress. Allow "Skip download for now".
- **Step 3: Test Recording:** "Say something." Record button → Transcribe → Show result. Prove it works.
- **Step 4: Hotkey Setup:** Show default (Win+Shift+R or similar). Allow customization. Explain Push-to-Talk vs Toggle.
- **Completion:** "All set!" button leads to the main window.

### Technical
- **State:** `Settings.HasCompletedOnboarding` boolean in DB.
- **UI:** Wizard layout (Next/Back/Skip buttons).
- **Component Reuse:** Embed the existing `ModelSelector`, `RecorderControl`, and `HotkeyEditor` components into the wizard pages to avoid code duplication.

## Acceptance Criteria (Verification Steps)

- **Verify First Launch:**
  - Clear any existing user settings or simulate a fresh install.
  - Launch the application.
  - Verify that the Onboarding Wizard appears instead of the main window.
  - Verify that the Welcome screen is visible.

- **Verify Model Selection (Tiny):**
  - Navigate to the Model Selection step in the wizard.
  - Select "Tiny (Fastest)".
  - Click "Next".
  - Verify that the application proceeds immediately to the Test Recording step (no download should occur).

- **Verify Model Selection (Base):**
  - Navigate to the Model Selection step.
  - Select "Base (Better Accuracy)".
  - Click "Next".
  - Verify that the download for the Base model starts.
  - Verify that a progress bar is displayed.
  - Verify that the "Next" button is disabled (or replaced by a "Skip" option) until the download completes.

- **Verify Test Recording Success:**
  - Navigate to the Test Recording step.
  - Click "Record Test".
  - Speak a phrase (e.g., "Hello World").
  - Click "Stop".
  - Verify that the transcription "Hello World" (or similar) appears on the screen.
  - Verify that the "Next" button becomes enabled.

- **Verify Wizard Completion:**
  - Navigate to the final Hotkey step.
  - Configure a hotkey.
  - Click "Finish".
  - Verify that the wizard closes.
  - Verify that the Main Window opens.
  - Check the database or settings file to confirm `HasCompletedOnboarding` is set to `true`.
