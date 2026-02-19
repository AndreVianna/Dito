using System;

namespace VivaVoz.Services.Audio;

public sealed class AudioRecordingStoppedEventArgs : EventArgs
{
    public AudioRecordingStoppedEventArgs(string filePath, TimeSpan duration)
    {
        FilePath = filePath;
        Duration = duration;
    }

    public string FilePath { get; }

    public TimeSpan Duration { get; }
}
