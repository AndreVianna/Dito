using AwesomeAssertions;
using VivaVoz.Services;
using Xunit;

namespace VivaVoz.Tests.Services;

public class TrayServiceTests {
    // ========== TrayIconState enum ==========

    [Fact]
    public void TrayIconState_ShouldHaveIdleValue() {
        var state = TrayIconState.Idle;

        state.Should().Be(TrayIconState.Idle);
    }

    [Fact]
    public void TrayIconState_ShouldHaveRecordingValue() {
        var state = TrayIconState.Recording;

        state.Should().Be(TrayIconState.Recording);
    }

    [Fact]
    public void TrayIconState_ShouldHaveTranscribingValue() {
        var state = TrayIconState.Transcribing;

        state.Should().Be(TrayIconState.Transcribing);
    }

    [Fact]
    public void TrayIconState_IdleShouldNotEqualRecording() {
        TrayIconState.Idle.Should().NotBe(TrayIconState.Recording);
    }

    // ========== TrayService.FormatTooltipText ==========

    [Fact]
    public void FormatTooltipText_WithNullTranscript_ShouldReturnDefaultText() {
        var result = TrayService.FormatTooltipText(null);

        result.Should().Be("VivaVoz — No speech detected.");
    }

    [Fact]
    public void FormatTooltipText_WithEmptyTranscript_ShouldReturnDefaultText() {
        var result = TrayService.FormatTooltipText(string.Empty);

        result.Should().Be("VivaVoz — No speech detected.");
    }

    [Fact]
    public void FormatTooltipText_WithShortTranscript_ShouldReturnFullText() {
        var result = TrayService.FormatTooltipText("Hello world");

        result.Should().Be("VivaVoz — Hello world");
    }

    [Fact]
    public void FormatTooltipText_WithLongTranscript_ShouldTruncateTo30Chars() {
        var transcript = "This is a very long transcript that should be truncated";

        var result = TrayService.FormatTooltipText(transcript);

        result.Should().Be("VivaVoz — This is a very long transcript...");
    }

    [Fact]
    public void FormatTooltipText_WithExactly30CharTranscript_ShouldNotTruncate() {
        var transcript = new string('a', 30); // exactly 30 chars

        var result = TrayService.FormatTooltipText(transcript);

        result.Should().Be($"VivaVoz — {transcript}");
        result.Should().NotContain("...");
    }

    // ========== TrayService.GetTooltipForState ==========

    [Fact]
    public void GetTooltipForState_WhenIdle_ShouldReturnIdleText() {
        var result = TrayService.GetTooltipForState(TrayIconState.Idle);

        result.Should().Be("VivaVoz");
    }

    [Fact]
    public void GetTooltipForState_WhenRecording_ShouldReturnRecordingText() {
        var result = TrayService.GetTooltipForState(TrayIconState.Recording);

        result.Should().Be("VivaVoz — Recording...");
    }

    [Fact]
    public void GetTooltipForState_WhenTranscribing_ShouldReturnTranscribingText() {
        var result = TrayService.GetTooltipForState(TrayIconState.Transcribing);

        result.Should().Be("VivaVoz — Transcribing...");
    }
}
