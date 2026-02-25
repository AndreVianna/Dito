# Delivery 2c — UX Polish (Pre-MVP)

## TASK-2c.1: Remove Storage Section from Settings

**Priority:** High  
**Effort:** Small  

### What
Remove the **Storage** section from the Settings screen (both `Location` and `Export Format` fields).

### Why
The Export Text and Export Audio buttons on the recording detail view already open a standard Save Dialog where the user picks the destination folder and file format. Having Storage Location and Export Format in Settings is duplicated, confusing, and potentially contradictory (Settings shows MP3 but Export Audio only offers WAV).

### Changes Required
1. **SettingsViewModel.cs** — Remove `StoragePath` and `ExportFormat` observable properties, their `OnChanged` partial methods, the `AvailableExportFormats` static list, and the loading lines in the constructor
2. **SettingsView.axaml** — Remove the Storage section (Location TextBox + Export Format ComboBox + section header)
3. **Settings entity** — Keep `StoragePath` in the data model (still needed internally for where recordings are saved). Remove `ExportFormat` property
4. **No migration needed** — Column can stay in DB, just remove the C# property. EF will ignore unmapped columns.

### Test Changes

**SettingsViewModelTests.cs — REMOVE (3 tests):**
- `Constructor_ShouldInitializeExportFormatFromSettings` — property removed from ViewModel
- `AvailableExportFormats_ShouldContainExpectedFormats` — static list removed
- `ExportFormat_WhenChanged_ShouldCallSaveSettings` — property removed

**SettingsViewModelTests.cs — KEEP (2 tests, they stay valid):**
- `Constructor_ShouldInitializeStoragePathFromSettings` — StoragePath stays in model, just hidden from UI
- `StoragePath_WhenChanged_ShouldCallSaveSettings` — still needs to persist correctly

**WAIT — if StoragePath is removed from ViewModel too**, move these 2 tests to a new **SettingsServiceTests.cs**:
```csharp
[Fact] Settings_ShouldHaveDefaultStoragePath()
[Fact] SaveSettings_WithCustomStoragePath_ShouldPersist()
```

### Acceptance Criteria
- [ ] Settings screen has no Storage section
- [ ] Export Audio still works via Save Dialog (user picks folder + format)
- [ ] Export Text still works via Save Dialog
- [ ] Internal storage path for recordings still works (default %LOCALAPPDATA%/VivaVoz)
- [ ] StoragePath still tested at service/entity level
- [ ] All remaining tests pass (3 tests removed, 0 broken)

---

## TASK-2c.2: Display Full Language Name Instead of ISO Code

**Priority:** High  
**Effort:** Small  

### What
Show full language names (e.g., "Portuguese", "English") instead of ISO codes ("pt", "en") in:
1. The **recording list** (left panel — shows "pt" next to duration)
2. The **recording detail view** (Language field shows "pt")

### Why
ISO codes are not user-friendly. Most users won't know that "pt" means Portuguese.

### Changes Required
1. **Create `LanguageHelper.cs`** (in Services/ or Helpers/) — static method `GetDisplayName(string isoCode)`:
   - Use `CultureInfo.GetCultureInfo(code).EnglishName` (returns "Portuguese", "English", etc.)
   - Handle edge cases: null/empty → "Unknown", "auto" → "Auto-detected", invalid code → return the code as-is (graceful fallback)
   - Wrap in try/catch since `CultureInfo` throws on invalid codes
2. **RecordingListItem template** (in MainView.axaml or wherever the list item is defined) — Use converter or bind through a display property
3. **RecordingDetailView** — Same: show `LanguageHelper.GetDisplayName(recording.LanguageCode)` instead of raw code
4. **Option A (converter):** Create `LanguageCodeToNameConverter : IValueConverter` and use in XAML bindings
5. **Option B (ViewModel property):** Add a `LanguageDisplayName` computed property to the recording view model

### New Tests
**LanguageHelperTests.cs** (new file):
```csharp
[Fact] GetDisplayName_WithPt_ShouldReturnPortuguese()
[Fact] GetDisplayName_WithEn_ShouldReturnEnglish()
[Fact] GetDisplayName_WithFr_ShouldReturnFrench()
[Fact] GetDisplayName_WithNull_ShouldReturnUnknown()
[Fact] GetDisplayName_WithEmpty_ShouldReturnUnknown()
[Fact] GetDisplayName_WithAuto_ShouldReturnAutoDetected()
[Fact] GetDisplayName_WithInvalidCode_ShouldReturnCodeAsIs()
```

### Acceptance Criteria
- [ ] Recording list shows full language name (e.g., "Portuguese" not "pt")
- [ ] Recording detail view shows full language name
- [ ] Unknown/null language shows "Unknown"
- [ ] Invalid codes show the code itself (graceful fallback)
- [ ] 7+ new tests, all passing
- [ ] All existing tests still pass

---

## TASK-2c.3: Tray Icon Reflects Recording Status

**Priority:** High  
**Effort:** Medium  

### What
Change the system tray icon to visually indicate the current app state:
- **Idle** — Default Bem-Te-Vi icon
- **Recording** — Red variant (red dot overlay on Bem-Te-Vi)
- **Transcribing** — Orange/blue variant (processing indicator)
- **Complete** — Green variant, reverts to idle after 3 seconds

### Why
When using VivaVoz via hotkey only (window closed/minimized), there's zero visual feedback. The user doesn't know if recording started, if transcription is running, or when text is ready.

### Changes Required
1. **Create icon variants** — 4 ICO files in `Assets/TrayIcons/`:
   - `tray-idle.ico` (existing Bem-Te-Vi)
   - `tray-recording.ico` (with red dot overlay)
   - `tray-transcribing.ico` (with orange/blue dot overlay)
   - `tray-ready.ico` (with green dot overlay)
   - Simple colored circle (8x8px) in bottom-right corner of the 16x16/32x32 icon
2. **Create `AppState` enum** — `Idle`, `Recording`, `Transcribing`, `Ready`
3. **Create `ITrayIconService`** interface + `TrayIconService` implementation:
   - `SetState(AppState state)` — updates tray icon
   - `SetStateTemporary(AppState state, TimeSpan duration)` — sets state then reverts to Idle
   - Uses a `DispatcherTimer` or `Task.Delay` for the revert
4. **Wire into existing flow:**
   - When `IAudioRecorder.StartAsync()` is called → `SetState(Recording)`
   - When recording stops and `ITranscriptionManager` begins → `SetState(Transcribing)`
   - When transcription completes → `SetStateTemporary(Ready, 3s)`
   - On any error/cancel → `SetState(Idle)`
5. **Update MainViewModel** (or wherever recording lifecycle is managed) to call `ITrayIconService` at each state transition

### New Tests
**TrayIconServiceTests.cs** (new file):
```csharp
[Fact] SetState_WithRecording_ShouldUpdateIcon()
[Fact] SetState_WithTranscribing_ShouldUpdateIcon()
[Fact] SetState_WithIdle_ShouldUseDefaultIcon()
[Fact] SetStateTemporary_WithReady_ShouldRevertToIdleAfterDuration()
[Fact] SetState_CalledDuringTemporary_ShouldCancelRevert()
```

**MainViewModel integration tests (update existing):**
```csharp
[Fact] RecordCommand_WhenStarted_ShouldSetTrayStateToRecording()
[Fact] RecordCommand_WhenStopped_ShouldSetTrayStateToTranscribing()
[Fact] Transcription_WhenComplete_ShouldSetTrayStateToReady()
[Fact] RecordCommand_WhenError_ShouldSetTrayStateToIdle()
```

### Acceptance Criteria
- [ ] Tray icon changes to recording state when hotkey starts recording
- [ ] Tray icon changes to transcribing state when transcription begins
- [ ] Tray icon changes to done/ready state when transcription completes
- [ ] Tray icon reverts to idle after 3 seconds
- [ ] Rapid state changes don't cause race conditions (cancel previous timer)
- [ ] Works correctly when main window is closed/minimized
- [ ] 9+ new tests, all passing
- [ ] All existing tests still pass

---

## TASK-2c.4: Auto-Copy Transcript to Clipboard on Completion

**Priority:** High  
**Effort:** Small  

### What
Automatically copy the completed transcript text to the system clipboard when transcription finishes, without requiring any user interaction.

### Why
The primary use case for hotkey-only users is: press hotkey → speak → press hotkey → paste. Currently they must open the app and click Copy. This removes that friction entirely.

### Changes Required
1. **Settings entity** — Add `AutoCopyToClipboard` (bool, default: `true`)
2. **Migration** — Add column to Settings table
3. **SettingsViewModel** — Add observable property + toggle in UI (checkbox in Settings under Transcription section)
4. **SettingsView.axaml** — Add "Auto-copy to clipboard" checkbox
5. **Transcription completion handler** (in MainViewModel or TranscriptionManager callback):
   - After saving transcript to database
   - If `Settings.AutoCopyToClipboard` is true → call `IClipboardService.SetTextAsync(transcriptText)`
   - `IClipboardService` already exists and is injected into MainViewModel
6. **Coordinate with TASK-2c.3** — The green "ready" tray icon signals "text is in your clipboard"

### New Tests
**SettingsViewModelTests.cs (add):**
```csharp
[Fact] Constructor_ShouldInitializeAutoCopyToClipboardFromSettings()
[Fact] AutoCopyToClipboard_WhenChanged_ShouldCallSaveSettings()
```

**MainViewModel transcription tests (add):**
```csharp
[Fact] Transcription_WhenCompleteAndAutoCopyEnabled_ShouldCopyToClipboard()
[Fact] Transcription_WhenCompleteAndAutoCopyDisabled_ShouldNotCopyToClipboard()
[Fact] Transcription_WhenCompleteWithEmptyTranscript_ShouldNotCopyToClipboard()
[Fact] Transcription_WhenClipboardFails_ShouldNotThrow()
```

### Acceptance Criteria
- [ ] After transcription completes, transcript text is automatically in clipboard (when enabled)
- [ ] User can immediately Ctrl+V the transcript without opening VivaVoz
- [ ] Works when app is minimized to tray
- [ ] Setting toggle in UI (default: on)
- [ ] Empty/null transcripts don't copy garbage to clipboard
- [ ] Clipboard failure doesn't crash the app
- [ ] 6+ new tests, all passing
- [ ] All existing tests still pass

---

## Summary

| Task | Tests Removed | Tests Added | Net Change |
|------|--------------|-------------|------------|
| 2c.1 | 3 | 2 | -1 |
| 2c.2 | 0 | 7+ | +7 |
| 2c.3 | 0 | 9+ | +9 |
| 2c.4 | 0 | 6+ | +6 |
| **Total** | **3** | **24+** | **+21** |

## PRD Updates Required

After implementation, update PRD.md:
1. **Section 7.4 Settings** — Remove "Storage location" and "Export defaults" lines. Add "Auto-copy to clipboard" toggle.
2. **Section 9 Settings entity** — Remove `ExportFormat`. Add `AutoCopyToClipboard (bool, default: true)`.
3. **Section 4.1 F4: Export** — Add "Auto-copy transcript to clipboard on transcription completion (configurable)"
4. **Section 7.1 System Tray** — Confirm tray icon states are implemented as specified
