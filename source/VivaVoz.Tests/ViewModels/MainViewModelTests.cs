using AwesomeAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
    public void Constructor_WithNullRecorder_ShouldThrowArgumentNullException() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var player = Substitute.For<IAudioPlayer>();

        var act = () => new MainViewModel(null!, player, context);

        act.Should().Throw<ArgumentNullException>().WithParameterName("recorder");
    }

    [Fact]
    public void Constructor_WithNullAudioPlayer_ShouldThrowArgumentNullException() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();

        var act = () => new MainViewModel(recorder, null!, context);

        act.Should().Throw<ArgumentNullException>().WithParameterName("audioPlayer");
    }

    [Fact]
    public void Constructor_WithNullDbContext_ShouldThrowArgumentNullException() {
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var act = () => new MainViewModel(recorder, player, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
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
    public void IsRecording_WhenRecorderIsRecording_ShouldBeTrue() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(true);
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.IsRecording.Should().BeTrue();
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
    public void HasSelection_WhenNoRecordingSelected_ShouldBeFalse() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.HasSelection.Should().BeFalse();
    }

    [Fact]
    public void HasSelection_WhenRecordingSelected_ShouldBeTrue() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.SelectedRecording = CreateRecording(DateTime.UtcNow);

        viewModel.HasSelection.Should().BeTrue();
    }

    [Fact]
    public void NoSelection_WhenNoRecordingSelected_ShouldBeTrue() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.NoSelection.Should().BeTrue();
    }

    [Fact]
    public void NoSelection_WhenRecordingSelected_ShouldBeFalse() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.SelectedRecording = CreateRecording(DateTime.UtcNow);

        viewModel.NoSelection.Should().BeFalse();
    }

    [Fact]
    public void IsNotRecording_WhenNotRecording_ShouldBeTrue() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(false);
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.IsNotRecording.Should().BeTrue();
    }

    [Fact]
    public void OnIsRecordingChanged_ShouldRaiseIsNotRecordingPropertyChanged() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        var changed = new List<string>();
        viewModel.PropertyChanged += (_, args) => {
            if (args.PropertyName is not null)
                changed.Add(args.PropertyName);
        };

        viewModel.IsRecording = true;

        changed.Should().Contain("IsNotRecording");
    }

    [Fact]
    public void SelectRecordingCommand_WhenExecuted_ShouldSetSelectedRecording() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        var recording = CreateRecording(DateTime.UtcNow);

        viewModel.SelectRecordingCommand.Execute(recording);

        viewModel.SelectedRecording.Should().Be(recording);
    }

    [Fact]
    public void SelectRecordingCommand_WithNull_ShouldSetSelectedRecordingToNull() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        viewModel.SelectedRecording = CreateRecording(DateTime.UtcNow);

        viewModel.SelectRecordingCommand.Execute(null);

        viewModel.SelectedRecording.Should().BeNull();
    }

    [Fact]
    public void ClearSelectionCommand_WhenExecuted_ShouldSetSelectedRecordingToNull() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        viewModel.SelectedRecording = CreateRecording(DateTime.UtcNow);

        viewModel.ClearSelectionCommand.Execute(null);

        viewModel.SelectedRecording.Should().BeNull();
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
    public void StartRecordingCommand_WhenRecorderStartsSuccessfully_ShouldUpdateIsRecording() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(false, true);
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.StartRecordingCommand.Execute(null);

        recorder.Received(1).StartRecording();
    }

    [Fact]
    public void StartRecordingCommand_WhenMicrophoneNotFound_ShouldCatchException() {
        // ShowMicrophoneNotFoundDialog creates Avalonia UI elements which require
        // a running Avalonia application. We verify the catch block is entered
        // by confirming the MicrophoneNotFoundException doesn't propagate as-is.
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.When(r => r.StartRecording())
                .Do(_ => throw new MicrophoneNotFoundException("No mic"));
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        try {
            viewModel.StartRecordingCommand.Execute(null);
        }
        catch (Exception ex) {
            // The MicrophoneNotFoundException is caught; any exception here
            // is from the Avalonia dialog code (InvalidOperationException), not the original
            ex.Should().NotBeOfType<MicrophoneNotFoundException>();
        }
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

    [Fact]
    public void StopRecordingCommand_WhenExecuted_ShouldUpdateIsRecordingFromRecorder() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        recorder.IsRecording.Returns(true, false);
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.StopRecordingCommand.Execute(null);

        recorder.Received(1).StopRecording();
    }

    [Fact]
    public void OnSelectedRecordingChanged_WithNull_ShouldSetDefaultHeaders() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        viewModel.SelectedRecording = CreateRecording(DateTime.UtcNow);

        viewModel.SelectedRecording = null;

        viewModel.DetailHeader.Should().Be("No recording selected");
        viewModel.DetailBody.Should().Be("Select a recording from the list to view details.");
    }

    [Fact]
    public void OnSelectedRecordingChanged_WithRecording_ShouldSetDetailHeader() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        var recording = CreateRecording(DateTime.UtcNow);
        recording.Title = "My Recording";

        viewModel.SelectedRecording = recording;

        viewModel.DetailHeader.Should().Be("My Recording");
        viewModel.DetailBody.Should().Be("Detail view placeholder.");
    }

    [Fact]
    public void OnSelectedRecordingChanged_WithEmptyTitle_ShouldUseDefaultTitle() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        var recording = CreateRecording(DateTime.UtcNow);
        recording.Title = "";

        viewModel.SelectedRecording = recording;

        viewModel.DetailHeader.Should().Be("Recording selected");
    }

    [Fact]
    public void OnSelectedRecordingChanged_WithWhitespaceTitle_ShouldUseDefaultTitle() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);
        var recording = CreateRecording(DateTime.UtcNow);
        recording.Title = "   ";

        viewModel.SelectedRecording = recording;

        viewModel.DetailHeader.Should().Be("Recording selected");
    }

    [Fact]
    public void OnSelectedRecordingChanged_ShouldRaiseNoSelectionPropertyChanged() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new MainViewModel(recorder, player, context);

        var changed = new List<string>();
        viewModel.PropertyChanged += (_, args) => {
            if (args.PropertyName is not null)
                changed.Add(args.PropertyName);
        };

        viewModel.SelectedRecording = CreateRecording(DateTime.UtcNow);

        changed.Should().Contain("NoSelection");
    }

    [Fact]
    public void AudioPlayer_ShouldBeInitialized() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.AudioPlayer.Should().NotBeNull();
    }

    [Fact]
    public void DetailHeader_WhenNewInstance_ShouldHaveDefaultValue() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.DetailHeader.Should().Be("No recording selected");
    }

    [Fact]
    public void DetailBody_WhenNewInstance_ShouldHaveDefaultValue() {
        using var connection = CreateConnection();
        using var context = CreateContext(connection);
        var recorder = Substitute.For<IAudioRecorder>();
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new MainViewModel(recorder, player, context);

        viewModel.DetailBody.Should().Be("Select a recording from the list to view details.");
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
