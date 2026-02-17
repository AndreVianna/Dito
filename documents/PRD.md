# Dito — Product Requirements Document

**Product:** Dito
**Tagline:** Said and done.
**Version:** 1.0 (MVP)
**Date:** 2026-02-17
**Authors:** Andre Vianna (Founder), Lola Lovelace (Product Lead)
**Company:** Casulo AI Labs

---

## Executive Summary

Dito is a $5 Windows desktop app that captures voice, transcribes it locally using Whisper, and exports recordings as audio or text. No cloud. No subscription. No account needed.

The market opportunity: Mac has 5+ polished voice-to-text tools (SuperWhisper, Auto, Wispr Flow). Windows has nothing comparable at an affordable price point. Wispr Flow ($81M funded) is entering Windows but is cloud-only and subscription-based, leaving the local-first, privacy-respecting niche wide open.

Dito fills that gap. Built with .NET 10 + Blazor Hybrid for portability. MVP in 4 weeks. $5 impulse buy. Microsoft Store + direct download.

*Dito e feito — said and done.*

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2026-02-17 | Lola Lovelace | Initial PRD — MVP scope, tech stack, data model |
| 0.2 | 2026-02-17 | Andre Vianna / Lola Lovelace | Resolved open questions: pricing ($5), format (MP3 default), distribution (Store + direct), Whisper bundling (tiny + on-demand) |
| 0.3 | 2026-02-17 | Lola Lovelace | Added executive summary, change log, index. Casulo document standard. |

---

## Index

1. [Overview](#1-overview)
2. [Problem Statement](#2-problem-statement)
3. [Target Users](#3-target-users)
4. [MVP Scope (v1.0)](#4-mvp-scope-v10)
5. [User Interface](#5-user-interface)
6. [Tech Stack](#6-tech-stack)
7. [Data Model](#7-data-model-draft)
8. [Future Versions](#8-future-versions-out-of-mvp-scope)
9. [Success Metrics](#9-success-metrics)
10. [Decisions Made](#10-decisions-made)
11. [Open Questions](#11-open-questions)

---

## 1. Overview

Dito is a Windows desktop application that captures voice input, transcribes it locally using Whisper, and lets users export recordings as audio files or text. No cloud dependency. No subscription. Your voice stays on your machine.

**Origin:** Inspired by SuperWhisper and Auto (Mac-only tools). No polished, local-first, affordable voice-to-text tool exists for Windows. Dito fills that gap.

## 2. Problem Statement

When people interact with AI or compose text, they type — and in doing so, they self-edit. They strip out context, reasoning, qualifiers, and intent. The result is compressed, imprecise input that produces generic output.

Voice preserves the "mess" — the qualifiers, the second-guessing, the *why* behind the *what*. That mess is what AI (and humans) need to produce meaningful results.

Mac users have multiple polished tools for this. **Windows users have nothing good.**

## 3. Target Users

### Primary
- **Knowledge workers on Windows** who interact with AI tools (ChatGPT, Copilot, Claude) and want richer input
- **Developers** who use coding agents and want to dictate intent rather than type compressed prompts

### Secondary
- **Writers and content creators** who think better out loud
- **Professionals** who need quick voice memos transcribed (meetings, ideas, notes)
- **Non-native English speakers** who express themselves more naturally by speaking

## 4. MVP Scope (v1.0)

### 4.1 Core Features

#### F1: Voice Capture
- Global hotkey to start/stop recording (configurable)
- Push-to-talk mode (hold key to record, release to stop)
- Toggle mode (press to start, press again to stop)
- Visual indicator showing recording state (system tray + overlay)
- Support for default system microphone

#### F2: Local Transcription
- On-device transcription using Whisper (no cloud)
- Multiple model sizes (tiny → large) — user selects based on speed/accuracy preference
- Language auto-detection
- Manual language selection override
- Transcription happens automatically after recording stops

#### F3: Recording Management (CRUD)
- List all recordings with metadata (date, duration, language, transcript preview)
- View full transcript for any recording
- Play back original audio
- Edit transcript text manually
- Delete recordings
- Search recordings by transcript content
- Sort by date, duration, or title

#### F4: Export
- Export as audio file (default: MP3; options: WAV, OGG)
- Export as text file (TXT/MD)
- Copy transcript to clipboard (one-click)
- Batch export selected recordings

### 4.2 Non-Functional Requirements

#### Performance
- Recording start latency: < 200ms from hotkey press
- Transcription speed: ≥ real-time (1 min audio ≤ 1 min processing) on modern hardware
- App startup: < 3 seconds
- Memory footprint: < 200MB idle, < 500MB during transcription

#### Privacy
- All processing local by default
- No telemetry without explicit opt-in
- No cloud calls in v1
- Audio files stored in user-controlled local directory

#### Compatibility
- Windows 10 21H2+ and Windows 11
- x64 architecture (ARM64 stretch goal)
- Minimum: 4GB RAM, 4-core CPU
- Recommended: 8GB RAM, 8-core CPU (for larger Whisper models)

#### Accessibility
- Keyboard-navigable UI
- Screen reader compatible
- High contrast theme support

## 5. User Interface

### 5.1 System Tray
- Dito lives in the system tray when not actively in use
- Tray icon shows recording state (idle / recording / transcribing)
- Right-click menu: Open Dito, Quick Record, Settings, Exit

### 5.2 Main Window
- **Recordings List** — Left panel, chronological, searchable
- **Detail View** — Right panel showing selected recording's transcript + audio player
- **Quick Actions** — Copy, Export, Delete, Edit

### 5.3 Recording Overlay
- Minimal floating indicator during recording (waveform + duration)
- Always-on-top, draggable, dismissable
- Click to stop recording

### 5.4 Settings
- Hotkey configuration
- Whisper model selection (with download manager)
- Audio input device selection
- Storage location
- Export defaults (format, output directory)
- Theme (light/dark/system)

## 6. Tech Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | .NET 10 + Blazor Hybrid (MAUI) |
| **UI** | Blazor components + MudBlazor or FluentUI |
| **Transcription** | whisper.cpp via Whisper.net (P/Invoke) |
| **Audio Capture** | NAudio or platform APIs |
| **Storage** | SQLite (via EF Core) |
| **Audio Playback** | NAudio |
| **Installer** | MSIX or WiX |

## 7. Data Model (Draft)

```
Recording
├── Id (GUID)
├── Title (auto-generated or user-set)
├── AudioFilePath (local path)
├── Transcript (text)
├── Language (detected or set)
├── Duration (TimeSpan)
├── CreatedAt (DateTime)
├── UpdatedAt (DateTime)
├── WhisperModel (which model was used)
└── Tags (optional, for organization)

Settings
├── HotkeyConfig
├── WhisperModelSize
├── AudioInputDevice
├── StoragePath
├── Theme
└── ExportDefaults
```

## 8. Future Versions (Out of MVP Scope)

### v2: AI Cleanup
- BYOK (Bring Your Own Key) — OpenAI, Anthropic, Gemini
- Mode system — custom AI cleanup prompts per task
- Clipboard auto-paste into active window

### v3: Smart Features
- App-aware mode switching
- Custom vocabulary / jargon dictionary
- Streaming transcription (real-time as you speak)

### v4: Platform Expansion
- Web companion (Blazor WebAssembly)
- Mac support (via MAUI)
- Team/enterprise features

## 9. Success Metrics

- **Build:** Working MVP in 4 weeks
- **Validate:** 100 beta users in first month
- **Revenue:** First paid download within 6 weeks of launch ($5 one-time, impulse price point)

## 10. Decisions Made

1. **Whisper model distribution** — Bundle smallest model (tiny). Larger models download on demand via in-app model manager.
2. **Audio format** — Default export: MP3. Configurable dropdown: MP3, WAV, OGG.
3. **Pricing** — $5 one-time. Gain clients first.
4. **Distribution** — Both Microsoft Store and direct download (dito-app.com).
5. **Domain** — dito-app.com (available, to be registered).

## 11. Open Questions

1. **Name trademark** — Need to check "Dito" availability

---

*Dito e feito.* 🎙️
