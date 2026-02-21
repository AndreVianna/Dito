namespace VivaVoz.Services;

/// <summary>
/// Clipboard service implementation using Avalonia's TopLevel clipboard API.
/// </summary>
[ExcludeFromCodeCoverage]
public class ClipboardService : IClipboardService {
    public async Task SetTextAsync(string text) {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is { } mainWindow) {
            var clipboard = TopLevel.GetTopLevel(mainWindow)?.Clipboard;
            if (clipboard is not null) {
                await clipboard.SetTextAsync(text);
            }
        }
    }
}
