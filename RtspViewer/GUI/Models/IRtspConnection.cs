using System;
using RtspClientSharp;

namespace RtspViewer.GUI.Models
{
    public interface IRtspConnection
    {
        event EventHandler<string> StatusChanged;

        IVideoSource VideoSource { get; }

        void Start(ConnectionParameters connectionParameters);
        void Stop();
    }
}