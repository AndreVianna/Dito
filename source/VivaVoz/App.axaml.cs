namespace VivaVoz;

[ExcludeFromCodeCoverage]
public partial class App : Application {
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted() {
        InitializeFileSystem();
        var dbContext = InitializeDatabase();
        var settingsService = new SettingsService(() => new AppDbContext());
        await settingsService.LoadSettingsAsync();
        var recorderService = new AudioRecorderService();
        var audioPlayerService = new AudioPlayerService();
        var modelManager = new WhisperModelManager();
        var whisperEngine = new WhisperTranscriptionEngine(modelManager);
        var transcriptionManager = new TranscriptionManager(whisperEngine, () => new AppDbContext());
        var clipboardService = new ClipboardService();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow {
                DataContext = new MainViewModel(recorderService, audioPlayerService, dbContext, transcriptionManager, clipboardService, settingsService)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void InitializeFileSystem() {
        var fileSystemService = new FileSystemService();
        FileSystemService.EnsureAppDirectories();
    }

    private static AppDbContext InitializeDatabase() {
        var dbContext = new AppDbContext();
        dbContext.Database.Migrate();
        return dbContext;
    }
}
