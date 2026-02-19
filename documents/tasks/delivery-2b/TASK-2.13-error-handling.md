# Task 2.13: Error Handling Framework

**Goal:** Implement a robust error handling strategy with three severity levels and user-friendly UI patterns.
**Part of:** Delivery 2b

## Context
Errors happen. VivaVoz needs to handle them gracefully, inform the user clearly, and offer a path forward. The PRD defines a specific strategy: Warning, Recoverable, and Catastrophic. This task implements that framework.

## Requirements

### Functional
- **Severity Levels:**
  - **Warning (Yellow):** Unexpected but non-blocking (e.g., transcription slightly inaccurate). Show a non-intrusive toast. Auto-dismiss.
  - **Recoverable (Orange):** Operation failed but app is stable (e.g., microphone busy, download failed). Show a modal dialog with specific actions (Retry, Skip, Help). Blocking until resolved.
  - **Catastrophic (Red):** App cannot continue (e.g., DB corruption, critical crash). Show a full-screen "Something went wrong" view. Offer to save logs and restart.
- **Logging:** All errors, regardless of severity, are logged to `%LOCALAPPDATA%/VivaVoz/logs/vivavoz.log`.
- **User Message:** Friendly, non-technical explanation. (e.g., "We couldn't access your microphone" vs "AccessViolationException").

### Technical
- **Interface:** `IErrorHandlerService`.
  - `HandleErrorAsync(Exception ex, ErrorSeverity severity, string userMessage, params Action[] recoveryOptions)`
- **Global Handler:** Catch unhandled exceptions at the `AppDomain` and `TaskScheduler` level.
- **UI Components:** Toast service, Modal dialog service, Full-screen error view.

## Acceptance Criteria (Verification Steps)

- **Verify Warning Toast:**
  - Simulate a non-critical error (e.g., "Low disk space warning") with `Severity.Warning`.
  - Verify that a toast notification appears with the message "Low disk space warning".
  - Verify that the notification automatically disappears after a short duration (e.g., 5 seconds).
  - Verify that the application continues to function normally without interruption.

- **Verify Recoverable Error Modal:**
  - Simulate a recoverable error (e.g., "Microphone in use") with `Severity.Recoverable`.
  - Verify that a modal dialog appears with the title "Microphone in use".
  - Verify that the dialog offers actionable buttons (e.g., "Retry", "Cancel").
  - Verify that the main window is inaccessible (blocked) until the dialog is closed.

- **Verify Catastrophic Error Screen:**
  - Simulate a critical error (e.g., "Database corruption") with `Severity.Catastrophic`.
  - Verify that the application navigates to a full-screen Error View.
  - Verify that the view displays "Something went wrong" (or similar user-friendly text).
  - Verify that the view offers a "Restart Application" button.
  - Verify that the view offers a "View Logs" button.
  - Verify that the main application functionality is disabled.

- **Verify Logging:**
  - Trigger errors of various severities (Warning, Recoverable, Catastrophic).
  - Locate the log file at `%LOCALAPPDATA%/VivaVoz/logs/vivavoz.log`.
  - Open the log file.
  - Verify that a log entry exists for each error.
  - Verify that each entry includes the timestamp, severity level, user message, and full stack trace.
