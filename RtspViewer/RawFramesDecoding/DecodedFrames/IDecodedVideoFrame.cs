﻿using System;

namespace RtspViewer.RawFramesDecoding.DecodedFrames
{
    public interface IDecodedVideoFrame
    {
        void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters);
    }
}