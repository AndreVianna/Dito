# Task 2.18: Installer & Packaging

**Goal:** Create a professional installer (WiX) and package the application for distribution, ensuring dependencies and models are correctly bundled.
**Part of:** Delivery 2b

## Context
"It works on my machine" isn't a product. VivaVoz needs to install cleanly on any Windows 10/11 PC.

## Requirements

### Functional
- **Installer Type:** MSI (WiX Toolset) for direct download. (MSIX will be a separate task for Store submission later if needed, but WiX is the primary distribution format for `vivavoz.app`).
- **Self-Contained:** Bundle the .NET 10 Runtime to ensure the app runs without pre-requisites.
- **Bundled Assets:** Ensure `whisper-tiny.bin` is included and placed in the correct `models/` subdirectory upon installation.
- **Shortcuts:** Create Desktop and Start Menu shortcuts.
- **Metadata:** Set correct Product Name ("VivaVoz"), Version, Publisher ("Casulo AI Labs"), and Icon.
- **Upgrade Logic:** Allow over-install (upgrade) without uninstalling first.

### Technical
- **Tool:** WiX Toolset v5 (or latest stable).
- **Build Command:** `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`.
- **WiX Project:** A `.wixproj` in the solution to wrap the publish output.
- **Registry:** No custom registry keys unless needed for file associations (future).

## Acceptance Criteria (Verification Steps)

- **Verify Clean Install:**
  - Given a clean Windows environment (or after removing previous versions).
  - Run the installer "VivaVoz.msi".
  - Verify that the installer completes successfully.
  - Verify that the application is installed to "%ProgramFiles%/VivaVoz" (or the specified target directory).
  - Verify that a shortcut "VivaVoz" is created on the Desktop.
  - Verify that the application launches successfully from the shortcut.

- **Verify Bundled Model Check:**
  - Given the application has just been installed.
  - Navigate to the installation folder (e.g., "%ProgramFiles%/VivaVoz").
  - Verify that the file "models/whisper-tiny.bin" exists.
  - Launch the application and perform a test transcription.
  - Verify that the application can load the Tiny model immediately without requiring an additional download.

- **Verify Uninstall:**
  - Given VivaVoz is installed.
  - Go to "Add or remove programs" in Windows Settings.
  - Select VivaVoz and click "Uninstall".
  - Verify that the uninstaller completes successfully.
  - Verify that the application files are removed from "%ProgramFiles%/VivaVoz".
  - Verify that the Desktop shortcut is removed.
  - Verify that user data in "%LOCALAPPDATA%/VivaVoz" is preserved (do not delete user recordings or settings).
