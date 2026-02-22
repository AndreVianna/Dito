namespace VivaVoz.Services;

/// <summary>
/// Abstraction over system clipboard for testability.
/// </summary>
public interface IClipboardService {
    Task SetTextAsync(string text);
}
