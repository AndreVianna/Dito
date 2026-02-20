using AwesomeAssertions;
using NSubstitute;
using VivaVoz.Models;
using VivaVoz.Services.Audio;
using VivaVoz.ViewModels;
using Xunit;

namespace VivaVoz.Tests.ViewModels;

public class AudioPlayerViewModelTests {
    [Fact]
    public void IsPlaying_WhenNewInstance_ShouldBeFalse() {
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new AudioPlayerViewModel(player);

        viewModel.IsPlaying.Should().BeFalse();
    }

    [Fact]
    public void HasAudio_WhenNewInstance_ShouldBeFalse() {
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new AudioPlayerViewModel(player);

        viewModel.HasAudio.Should().BeFalse();
    }

    [Fact]
    public void CurrentPosition_WhenNewInstance_ShouldBeZero() {
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new AudioPlayerViewModel(player);

        viewModel.CurrentPosition.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void TotalDuration_WhenNewInstance_ShouldBeZero() {
        var player = Substitute.For<IAudioPlayer>();

        var viewModel = new AudioPlayerViewModel(player);

        viewModel.TotalDuration.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void StopCommand_WhenExecuted_ShouldResetStateAndCallPlayerStop() {
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new AudioPlayerViewModel(player) {
            IsPlaying = true,
            CurrentPosition = TimeSpan.FromSeconds(5),
            TotalDuration = TimeSpan.FromSeconds(10),
            Progress = 0.5
        };

        viewModel.StopCommand.Execute(null);

        viewModel.IsPlaying.Should().BeFalse();
        viewModel.CurrentPosition.Should().Be(TimeSpan.Zero);
        viewModel.Progress.Should().Be(0);
        player.Received(1).Stop();
    }

    [Fact]
    public void TogglePlayPauseCommand_WhenNoAudioLoaded_ShouldNotCallPlayer() {
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new AudioPlayerViewModel(player);

        viewModel.TogglePlayPauseCommand.Execute(null);

        player.DidNotReceiveWithAnyArgs().Play(default!);
        player.DidNotReceive().Pause();
    }

    [Fact]
    public void LoadRecording_WithNull_ShouldClearAudioState() {
        var player = Substitute.For<IAudioPlayer>();
        var viewModel = new AudioPlayerViewModel(player) {
            HasAudio = true,
            TotalDuration = TimeSpan.FromSeconds(10)
        };

        viewModel.LoadRecording((Recording?)null);

        viewModel.HasAudio.Should().BeFalse();
        viewModel.TotalDuration.Should().Be(TimeSpan.Zero);
    }
}
