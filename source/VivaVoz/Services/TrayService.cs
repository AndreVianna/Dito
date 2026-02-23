using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Serilog;
using VivaVoz.Services.Audio;
using VivaVoz.Services.Transcription;

namespace VivaVoz.Services;

public class TrayService : ITrayService {
    private readonly IClassicDesktopStyleApplicationLifetime _desktop;
    private readonly IAudioRecorder _recorder;
    private readonly ITranscriptionManager _transcriptionManager;
    private TrayIcon? _trayIcon;
    private NativeMenuItem? _toggleRecordingItem;
    private TrayIconState _currentState = TrayIconState.Idle;
    private int _activeTranscriptions;
    private WindowIcon? _idleIcon;
    private WindowIcon? _activeIcon;

    private const string IdleIconUri = "avares://VivaVoz/Assets/vivavoz-16x16.png";
    private const string ActiveIconUri = "avares://VivaVoz/Assets/vivavoz-mono-16x16.png";

    /// <summary>
    /// The current tray icon state. Exposed as <c>internal</c> for unit testing.
    /// </summary>
    internal TrayIconState CurrentState => _currentState;

    /// <summary>
    /// The number of in-flight transcriptions. Exposed as <c>internal</c> for unit testing.
    /// </summary>
    internal int ActiveTranscriptions => _activeTranscriptions;

    public TrayService(
        IClassicDesktopStyleApplicationLifetime desktop,
        IAudioRecorder recorder,
        ITranscriptionManager transcriptionManager) {
        _desktop = desktop ?? throw new ArgumentNullException(nameof(desktop));
        _recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
        _transcriptionManager = transcriptionManager ?? throw new ArgumentNullException(nameof(transcriptionManager));
    }

    [ExcludeFromCodeCoverage(Justification = "Requires Avalonia platform and AssetLoader at runtime.")]
    public void Initialize() {
        _idleIcon = LoadIcon(IdleIconUri);
        _activeIcon = LoadIcon(ActiveIconUri);

        _toggleRecordingItem = new NativeMenuItem { Header = "Start Recording" };
        _toggleRecordingItem.Click += OnToggleRecordingClicked;

        var openItem = new NativeMenuItem { Header = "Open VivaVoz" };
        openItem.Click += (_, _) => ShowMainWindow();

        var settingsItem = new NativeMenuItem { Header = "Settings" };
        settingsItem.Click += (_, _) => ShowMainWindow();

        var exitItem = new NativeMenuItem { Header = "Exit" };
        exitItem.Click += (_, _) => _desktop.Shutdown();

        var menu = new NativeMenu();
        menu.Items.Add(_toggleRecordingItem);
        menu.Items.Add(openItem);
        menu.Items.Add(settingsItem);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(exitItem);

        _trayIcon = new TrayIcon {
            Icon = _idleIcon,
            ToolTipText = "VivaVoz",
            Menu = menu,
            IsVisible = true
        };
        _trayIcon.Clicked += (_, _) => ToggleMainWindowVisibility();

        _recorder.RecordingStarted += OnRecordingStarted;
        _recorder.RecordingStopped += OnRecordingStopped;
        _transcriptionManager.TranscriptionCompleted += OnTranscriptionCompleted;

        Log.Information("[TrayService] Tray icon initialized.");
    }

    public void SetState(TrayIconState state) {
        _currentState = state;
        if (_trayIcon is null) return;

        _trayIcon.Icon = state == TrayIconState.Idle ? _idleIcon : _activeIcon;
        _trayIcon.ToolTipText = GetTooltipForState(state);

        if (_toggleRecordingItem is not null) {
            _toggleRecordingItem.Header = state == TrayIconState.Recording
                ? "Stop Recording"
                : "Start Recording";
        }

        Log.Debug("[TrayService] State changed to {State}.", state);
    }

    public void ShowTranscriptionComplete(string? transcript) {
        if (_trayIcon is null) return;
        if (!ShouldShowTranscriptionNotification(_desktop.MainWindow)) return;

        _trayIcon.ToolTipText = FormatTooltipText(transcript);
        Log.Information("[TrayService] Transcription complete notification shown.");
    }

    public void Dispose() {
        _recorder.RecordingStarted -= OnRecordingStarted;
        _recorder.RecordingStopped -= OnRecordingStopped;
        _transcriptionManager.TranscriptionCompleted -= OnTranscriptionCompleted;
        _trayIcon?.Dispose();
        _trayIcon = null;
        _idleIcon = null;
        _activeIcon = null;
    }

    // ── Internal logic handlers (testable without Avalonia) ────────────────────

    /// <summary>
    /// Handles recording started: transitions state to <see cref="TrayIconState.Recording"/>.
    /// Exposed as <c>internal</c> for unit testing via <c>InternalsVisibleTo</c>.
    /// </summary>
    internal void HandleRecordingStarted() {
        SetState(TrayIconState.Recording);
    }

    /// <summary>
    /// Handles recording stopped: increments in-flight transcriptions and transitions
    /// state to <see cref="TrayIconState.Transcribing"/>.
    /// Exposed as <c>internal</c> for unit testing via <c>InternalsVisibleTo</c>.
    /// </summary>
    internal void HandleRecordingStopped() {
        Interlocked.Increment(ref _activeTranscriptions);
        SetState(TrayIconState.Transcribing);
    }

    /// <summary>
    /// Handles transcription completion: decrements in-flight counter and returns to
    /// <see cref="TrayIconState.Idle"/> when no more active transcriptions remain.
    /// Exposed as <c>internal</c> for unit testing via <c>InternalsVisibleTo</c>.
    /// </summary>
    internal void HandleTranscriptionCompleted(bool success, string? transcript) {
        var remaining = Interlocked.Decrement(ref _activeTranscriptions);

        if (remaining <= 0) {
            _activeTranscriptions = 0;
            SetState(TrayIconState.Idle);
        }

        if (success) {
            ShowTranscriptionComplete(transcript);
        }
    }

    /// <summary>
    /// Returns <c>true</c> when the main window is hidden and a tray notification
    /// (tooltip update) should be shown. Exposed as <c>internal</c> for unit testing.
    /// </summary>
    internal static bool ShouldShowTranscriptionNotification(Window? window)
        => window is not null && !window.IsVisible;

    // ── Avalonia event handlers (excluded from code coverage) ─────────────────

    [ExcludeFromCodeCoverage(Justification = "Dispatches to UI thread; tested via HandleRecordingStarted.")]
    private void OnRecordingStarted(object? sender, EventArgs e)
        => Avalonia.Threading.Dispatcher.UIThread.Post(HandleRecordingStarted);

    [ExcludeFromCodeCoverage(Justification = "Dispatches to UI thread; tested via HandleRecordingStopped.")]
    private void OnRecordingStopped(object? sender, AudioRecordingStoppedEventArgs e)
        => Avalonia.Threading.Dispatcher.UIThread.Post(HandleRecordingStopped);

    [ExcludeFromCodeCoverage(Justification = "Dispatches to UI thread; tested via HandleTranscriptionCompleted.")]
    private void OnTranscriptionCompleted(object? sender, TranscriptionCompletedEventArgs e) {
        var success = e.Success;
        var transcript = e.Transcript;
        Avalonia.Threading.Dispatcher.UIThread.Post(() => HandleTranscriptionCompleted(success, transcript));
    }

    [ExcludeFromCodeCoverage(Justification = "UI event handler; touches Avalonia recorder/controls.")]
    private void OnToggleRecordingClicked(object? sender, EventArgs e) {
        if (_currentState == TrayIconState.Recording) {
            _recorder.StopRecording();
        }
        else {
            try {
                _recorder.StartRecording();
            }
            catch (Exception ex) {
                Log.Error(ex, "[TrayService] Failed to start recording from tray.");
            }
        }
    }

    [ExcludeFromCodeCoverage(Justification = "Requires live Avalonia Window.")]
    private void ShowMainWindow() {
        var window = _desktop.MainWindow;
        if (window is null) return;

        window.Show();
        window.WindowState = WindowState.Normal;
        window.Activate();
    }

    [ExcludeFromCodeCoverage(Justification = "Requires live Avalonia Window.")]
    private void ToggleMainWindowVisibility() {
        var window = _desktop.MainWindow;
        if (window is null) return;

        if (window.IsVisible)
            window.Hide();
        else
            ShowMainWindow();
    }

    [ExcludeFromCodeCoverage(Justification = "Requires Avalonia AssetLoader at runtime.")]
    private static WindowIcon LoadIcon(string avaloniaUri) {
        var uri = new Uri(avaloniaUri);
        using var stream = AssetLoader.Open(uri);
        return new WindowIcon(new Bitmap(stream));
    }

    // ── Testable static helpers ────────────────────────────────────────────────

    public static string FormatTooltipText(string? transcript) {
        if (string.IsNullOrWhiteSpace(transcript))
            return "VivaVoz — No speech detected.";

        if (transcript.Length <= 30)
            return $"VivaVoz — {transcript}";

        return $"VivaVoz — {transcript[..30]}...";
    }

    public static string GetTooltipForState(TrayIconState state) => state switch {
        TrayIconState.Recording => "VivaVoz — Recording...",
        TrayIconState.Transcribing => "VivaVoz — Transcribing...",
        _ => "VivaVoz"
    };
}
