using System;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer.GUI
{
    interface IAudioSource
    {
        event EventHandler<IDecodedAudioFrame> FrameReceived;
    }
}
