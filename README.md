# VivaVoz

**Your voice, alive.** 🎙️

Voice-to-text for Windows. Record your voice, transcribe locally with Whisper, manage and export your recordings.

*Portuguese: "viva" (alive) + "voz" (voice)*

## Status

🚧 **Delivery 1a — Complete** (Foundation)
- Recording and playback working
- SQLite persistence
- 50 unit tests, all passing
- 0 errors, 0 warnings

⏳ **Delivery 1b — Next** (Transcription)
- Whisper integration for local transcription
- Transcript display and clipboard copy
- Settings persistence

## Features (Current)

- 🎤 Audio recording (WAV, 16kHz mono)
- ▶️ Playback with play/pause/stop
- 📋 Recording management (list, detail view, CRUD)
- 💾 SQLite storage with EF Core
- 📁 Organized file system (`%LOCALAPPDATA%/VivaVoz/`)
- 📝 Structured logging with Serilog

## Tech Stack

| Layer | Technology |
|-------|------------|
| **Framework** | .NET 10 |
| **UI** | Avalonia UI + Fluent Theme |
| **MVVM** | CommunityToolkit.Mvvm |
| **Audio** | NAudio |
| **Database** | SQLite via EF Core |
| **Logging** | Serilog (file sink) |
| **Testing** | xUnit + NSubstitute + AwesomeAssertions |

## Project Structure

```
source/
├── VivaVoz/
│   ├── Constants/       # App-wide constants (file paths)
│   ├── Data/            # EF Core DbContext + migrations
│   ├── Models/          # Domain models (Recording, Settings)
│   ├── Services/        # Business logic + audio services
│   ├── ViewModels/      # MVVM ViewModels
│   └── Views/           # Avalonia XAML views
└── VivaVoz.Tests/
    ├── Constants/       # FilePaths tests
    ├── Data/            # DbContext tests
    ├── Models/          # Model tests
    ├── Services/        # Service + audio tests
    └── ViewModels/      # ViewModel tests
```

## Building

```bash
cd source
dotnet restore
dotnet build
dotnet test
```

## Roadmap

| Delivery | Scope | Status |
|----------|-------|--------|
| **1a** | Foundation — recording, playback, persistence | ✅ Complete |
| **1b** | Transcription — Whisper, transcript display | ⏳ Next |
| **2a** | Features — search, tags, editing | 📋 Planned |
| **2b** | Export & Ship — MP3/TXT export, installer | 📋 Planned |

## By

[Andre Vianna](https://github.com/AndreVianna) & [Casulo AI Labs](https://casuloailabs.com)

## License

*TBD*
