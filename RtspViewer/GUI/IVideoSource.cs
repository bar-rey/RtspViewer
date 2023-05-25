using System;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer.GUI
{
    public interface IVideoSource
    {
        event EventHandler<IDecodedVideoFrame> FrameReceived;
    }
}