using AwesomeAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using VivaVoz.Data;
using VivaVoz.Models;
using VivaVoz.Services.Audio;
using VivaVoz.ViewModels;
using Xunit;

namespace VivaVoz.Tests.ViewModels;

public class MainViewModelTests {
    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var act = () => new MainViewModel(recorder, player, context);

        act.Should().NotThrow();
    }

    [Fact]
    public void IsRecording_WhenRecorderIsNotRecording_ShouldBeFalse() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(false);
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.IsRecording.Should().BeFalse();
    }

    [Fact]
    public void SelectedRecording_WhenNewInstance_ShouldBeNull() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.SelectedRecording.Should().BeNull();
    }

    [Fact]
    public void Recordings_WhenNewInstance_ShouldBeInitialized() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.Recordings.Should().NotBeNull();
    }

    [Fact]
    public void Recordings_WhenDatabaseHasRecordings_ShouldBeOrderedByCreatedAtDescending() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);

        var now = DateTime.UtcNow;
        var older = CreateRecording(now.AddMinutes(-10));
        var middle = CreateRecording(now.AddMinutes(-5));
        var newest = CreateRecording(now);

        context.Recordings.AddRange(older, middle, newest);
        context.SaveChanges();

        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.Recordings.Select(r => r.Id).Should().Equal(newest.Id, middle.Id, older.Id);
    }

    [Fact]
    public void SelectedRecording_WhenChanged_ShouldRaisePropertyChanged() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        var recording = CreateRecording(DateTime.UtcNow);

        var changed = new List<string>();
        viewModel.PropertyChanged += (_, args) => {
            if (args.PropertyName is not null)
                changed.Add(args.PropertyName);
        };

        viewModel.SelectedRecording = recording;

        changed.Should().Contain(nameof(MainViewModel.SelectedRecording));
        changed.Should().Contain(nameof(MainViewModel.HasSelection));
    }

    [Fact]
    public void StartRecordingCommand_WhenExecuted_ShouldCallRecorderStartRecording() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(false);
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.StartRecordingCommand.Execute(null);

        recorder.Received(1).StartRecording();
    }

    [Fact]
    public void StopRecordingCommand_WhenExecuted_ShouldCallRecorderStopRecording() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(false);
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.StopRecordingCommand.Execute(null);

        recorder.Received(1).StopRecording();
    }

    private static SqliteConnection CreateConnection() {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        return connection;
    }

    private static AppDbContext CreateContext(SqliteConnection connection) {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static Recording CreateRecording(DateTime createdAt) => new() {
        Id = Guid.NewGuid(),
        Title = "Test",
        AudioFileName = "file.wav",
        Status = RecordingStatus.Complete,
        Language = "en",
        Duration = TimeSpan.FromSeconds(10),
        CreatedAt = createdAt,
        UpdatedAt = createdAt,
        WhisperModel = "tiny",
        FileSize = 10
    };
}
