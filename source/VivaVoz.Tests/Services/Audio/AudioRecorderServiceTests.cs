using AwesomeAssertions;
using VivaVoz.Services.Audio;
using Xunit;

namespace VivaVoz.Tests.Services.Audio;

public class AudioRecorderServiceTests {
    [Fact]
    public void IsRecording_WhenNewInstance_ShouldBeFalse() {
        var service = new AudioRecorderService();

        service.IsRecording.Should().BeFalse();
    }

    [Fact]
    public void StopRecording_WhenNotRecording_ShouldNotThrow() {
        var service = new AudioRecorderService();

        var act = service.StopRecording;

        act.Should().NotThrow();
    }
}
