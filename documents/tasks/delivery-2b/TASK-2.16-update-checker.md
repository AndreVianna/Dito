# Task 2.16: Update Checker

**Goal:** Implement a lightweight, non-intrusive update check mechanism for direct download users.
**Part of:** Delivery 2b

## Context
Since we distribute via direct download (in addition to the Store), we need a way to tell users about bug fixes and new features. We respect their autonomy: no forced updates, no silent background downloads unless they ask for it.

## Requirements

### Functional
- **Check on Launch:** Silently check `vivavoz.app/version.json` (or configured URL) on startup.
- **Banner Notification:** If a newer version exists, show a dismissible banner at the top of the Main Window: "Update available (v1.1). [Update Now] [Later]"
- **Action - Update Now:** Open the download page in the default browser.
- **Setting - Auto-Update:** Opt-in checkbox in Settings: "Check for updates automatically". Default: False (per PRD v0.12, wait, PRD says "Auto-update setting (off by default): User can enable in Settings... When enabled: download happens in background").
  - *Correction based on PRD 0.12:* "Direct download opt-in auto-update (off by default)". Let's stick to the banner for MVP simplicity as described in the "Direct Download Update Flow" section: "If newer version available → non-intrusive banner... When enabled: download happens in background".
  - *Refinement:* For this task, let's implement the **Check + Banner + Browser Link**. Full background auto-update/installer integration is complex; the PRD says "When enabled: download happens in background, installs on next app restart". We will implement the check and the *hooks* for auto-update, but the primary MVP delivery is the notification.
- **No Interruption:** Never block the user from recording.

### Technical
- **Service:** `IUpdateService`.
  - `CheckForUpdateAsync()` returns `UpdateInfo` (Version, URL, Changelog).
- **Endpoint:** `https://vivavoz.app/version.json` (structure: `{ "version": "1.1.0", "url": "...", "notes": "..." }`).
- **Logic:** Compare `Assembly.GetExecutingAssembly().GetName().Version` with remote version.

## Acceptance Criteria (Verification Steps)

- **Verify Update Available:**
  - Mock or configure the remote `version.json` endpoint to report a version higher than the current assembly version (e.g., Remote: 1.1.0 vs Local: 1.0.0).
  - Launch the application.
  - Verify that a non-intrusive banner appears at the top of the main window.
  - Verify that the banner text states "Update available (v1.1)" (or similar).
  - Verify that a "Download" button is visible and functional (opens the download URL).

- **Verify No Update:**
  - Mock or configure the remote `version.json` to report a version equal to or lower than the current assembly version.
  - Launch the application.
  - Verify that no update banner appears.

- **Verify Update Check Error (Offline/Failure):**
  - Disconnect the network or simulate an unreachable update endpoint.
  - Launch the application.
  - Verify that the application starts normally without any error dialogs or notifications.
  - Verify (via logs) that the update check failed silently.
