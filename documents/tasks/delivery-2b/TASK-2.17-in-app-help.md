# Task 2.17: In-App Help & FAQ

**Goal:** Create a built-in help system to answer common questions and guide users without leaving the app.
**Part of:** Delivery 2b

## Context
Users should be able to solve basic problems without searching the web. An offline-first app deserves offline-first help.

## Requirements

### Functional
- **Help Button:** Accessible from the main UI (e.g., "?" icon or Settings menu).
- **Getting Started:** A brief "How to use VivaVoz" section (Record -> Transcribe -> Export).
- **FAQ:** Top 5 questions:
  1. "Where are my files stored?"
  2. "Which model should I use?"
  3. "How do I change the hotkey?"
  4. "My microphone isn't working."
  5. "How do I uninstall?"
- **Support Links:** "Visit Website" and "Email Support" buttons.
- **Offline:** Content must be embedded in the app, not fetched from the web.

### Technical
- **UI:** A dedicated `HelpView` or `AboutDialog`.
- **Format:** Use a Markdown renderer (Avalonia.Markdown) for clean, formatted text, or a simple `ItemsControl` for the FAQ list.
- **Hyperlinks:** `Process.Start` to open the website or default mail client (`mailto:`).

## Acceptance Criteria (Verification Steps)

- **Verify Open Help:**
  - Launch the application and ensure the main window is visible.
  - Click the "Help" button (or "?" icon).
  - Verify that the Help window or dialog opens.
  - Verify that the "Getting Started" section is visible and readable.

- **Verify FAQ Content:**
  - Navigate to the FAQ section in the Help window.
  - Expand or locate the question "Where are my files stored?".
  - Verify that the answer displayed mentions "%LOCALAPPDATA%/VivaVoz/" (or the correct path).
  - Verify that other FAQ items are present and expandable.

- **Verify Contact Support:**
  - Open the Help window.
  - Click the "Email Support" link or button.
  - Verify that the default email client opens a new draft.
  - Verify that the "To" field is pre-filled with "support@casuloailabs.com".
