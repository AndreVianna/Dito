# Task 2.10: Text Export

**Goal:** Enable users to export transcripts as plain text (.txt) or Markdown (.md) files.
**Part of:** Delivery 2b

## Context
Users capture voice to create content. They need to get the text out of VivaVoz and into their writing tools (Obsidian, Word, VS Code, etc.). This task handles the export of the transcription data.

## Requirements

### Functional
- **Export Button:** A distinct "Export Text" button or dropdown option in the detail view.
- **Format Selection:** Support .txt (Plain Text) and .md (Markdown).
- **Save Dialog:** Use the native OS "Save File" dialog.
- **Content Formatting:**
  - **TXT:** Just the raw transcript text.
  - **MD:** formatted with title, date, duration metadata, and the transcript body.
- **Default Filename:** Suggest based on recording title (e.g., `VivaVoz_Idea_Draft.md`).
- **Error Handling:** Standard file system errors (permission denied, disk full).

### Technical
- **Interface:** `ITextExporter` service.
  - `ExportTextAsync(Recording recording, string destinationPath, TextFormat format)`
- **Library:** Standard .NET `System.IO`.
- **UI:** Avalonia `SaveFileDialog`.

## Acceptance Criteria (Verification Steps)

- **Verify Export as Markdown:**
  - Create a recording titled "Project Brainstorm" with the transcript "Make sure to include the blue button."
  - Click "Export Text" in the detail view.
  - Select "Markdown (.md)" from the format options.
  - Choose a destination (e.g., `C:/Docs/Brainstorm.md`).
  - Verify that the file `C:/Docs/Brainstorm.md` is created.
  - Verify that the file content starts with "# Project Brainstorm".
  - Verify that the file content contains the transcript text "Make sure to include the blue button."
  - Verify that a success notification appears.

- **Verify Export as Plain Text:**
  - Create a recording titled "Quick Note" with the transcript "Buy milk."
  - Click "Export Text".
  - Select "Text (.txt)" from the format options.
  - Choose a destination (e.g., `C:/Docs/Note.txt`).
  - Verify that the file `C:/Docs/Note.txt` is created.
  - Verify that the file content consists exactly of "Buy milk." (without additional Markdown metadata).
  - Verify that a success notification appears.

- **Verify Export Cancellation:**
  - Open the "Export Text" dialog.
  - Click "Cancel".
  - Verify that no file is created at any destination.
  - Verify that no error message or notification is shown.
