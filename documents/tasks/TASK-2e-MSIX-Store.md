# Delivery 2e: MSIX Packaging & Microsoft Store Submission

**Branch:** `delivery-2e`
**Base:** `main` (after 2d merge)
**Goal:** Package VivaVoz for Microsoft Store distribution + direct download

---

## Tasks

### TASK-2e.1: MSIX Project Setup
**Priority:** High | **Estimate:** 2h

Create Windows Application Packaging Project (WAP) in the solution.

**Changes:**
- Add WAP project (`VivaVoz.Package`)
- Configure `Package.appxmanifest`:
  - Display Name: "VivaVoz"
  - Publisher: "Casulo AI Labs"
  - Description: "Your voice, alive. Local speech-to-text powered by Whisper AI."
  - Logo assets (see 2e.2)
  - Capabilities: `microphone` (audio recording)
  - Min version: Windows 10 1809 (17763)
- Reference main VivaVoz project
- Self-contained publish: `win-x64`, .NET 10
- Build command produces `.msix` or `.msixbundle`

**Acceptance Criteria:**
- [ ] WAP project builds successfully
- [ ] `dotnet publish` produces MSIX package
- [ ] Package installs on clean Windows 10/11
- [ ] App launches from Start Menu after install

---

### TASK-2e.2: Store Assets
**Priority:** High | **Estimate:** 2h

Generate all required visual assets from the Bem-Te-Vi icon.

**Required sizes (Store Policies):**
- Store Logo: 300x300
- App Icon: 150x150, 44x44, 16x16 (+ scale variants 100%, 125%, 150%, 200%, 400%)
- Wide Tile: 310x150
- Large Tile: 310x310
- Splash Screen: 620x300
- Badge Logo: 24x24 (monochrome)
- Screenshots: at least 1 (1366x768 or 1920x1080)

**Source:** `documents/branding/` (existing Bem-Te-Vi assets)

**Acceptance Criteria:**
- [ ] All required sizes generated
- [ ] Assets referenced in Package.appxmanifest
- [ ] No placeholder/default logos in installed app

---

### TASK-2e.3: Bundle Whisper Base Model
**Priority:** High | **Estimate:** 1h

Include `ggml-base.bin` in the MSIX package so the app works offline immediately.

**Changes:**
- Add `ggml-base.bin` to publish output (Content, CopyToOutputDirectory)
- Place in `models/` subdirectory within package
- First-run detection: if model exists in package dir, copy to `%LOCALAPPDATA%/VivaVoz/models/` (or reference directly)
- Verify app uses bundled model without download prompt

**Acceptance Criteria:**
- [ ] MSIX includes Base model (~150MB)
- [ ] First launch transcribes without internet
- [ ] Onboarding wizard shows Base as already installed

---

### TASK-2e.4: Signing & Store Submission
**Priority:** High | **Estimate:** 2h

Sign the MSIX and submit to Microsoft Partner Center.

**Prerequisites:**
- Microsoft Developer account ($19 one-time) — Andre to register
- Privacy Policy URL at vivavoz.app/privacy
- App listing content (description, screenshots, category)

**Changes:**
- Reserve app name "VivaVoz" in Partner Center
- Upload MSIX package
- Fill store listing:
  - Category: Productivity > Voice Recognition
  - Price: $4.99 USD
  - Markets: All
  - Age rating: Everyone
  - Privacy policy: https://vivavoz.app/privacy
- Submit for certification

**Acceptance Criteria:**
- [ ] App name reserved
- [ ] MSIX uploaded and validated by Partner Center
- [ ] Store listing complete with screenshots
- [ ] Submitted for certification review

---

### TASK-2e.5: Direct Download (WiX MSI)
**Priority:** Medium | **Estimate:** 3h

Alternative distribution via vivavoz.app (for users who prefer not to use the Store).

**Changes:**
- WiX v5 project (`VivaVoz.Installer`)
- Self-contained publish bundled into MSI
- Desktop + Start Menu shortcuts
- Bundled Base model
- Upgrade logic (over-install without uninstall)
- Uninstall preserves `%LOCALAPPDATA%/VivaVoz/` (user data)

**Acceptance Criteria:**
- [ ] MSI installs cleanly on Windows 10/11
- [ ] Shortcuts created
- [ ] Base model bundled
- [ ] Upgrade over previous version works
- [ ] Uninstall removes app but keeps user data

---

## Task Order
1. **2e.1** (MSIX project) — foundation
2. **2e.2** (Store assets) — needed for manifest
3. **2e.3** (bundle model) — needed for testing
4. **2e.4** (signing + submission) — the goal
5. **2e.5** (WiX MSI) — parallel/after Store submission

## Dependencies
- Andre: Microsoft Developer account registration
- Andre: Privacy policy page on vivavoz.app
- Andre: Screenshots from running app on Windows

## Package Size Estimate
- .NET 10 self-contained: ~80MB
- Whisper Base model: ~150MB
- App code + assets: ~5MB
- **Total MSIX: ~235MB**

---

*The bird is ready to fly. 🐦*
