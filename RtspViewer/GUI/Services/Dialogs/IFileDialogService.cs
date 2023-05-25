using RtspViewer.GUI.Services.Dialogs.Base;

namespace RtspViewer.GUI.Services.Dialogs
{
    public interface IFileDialogService : IDialogService
    {
        string SelectedFile { get; }
        bool SelectFileDialog(string defaultExt = "");
    }
}
