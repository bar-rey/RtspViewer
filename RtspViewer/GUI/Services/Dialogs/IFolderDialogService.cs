using RtspViewer.GUI.Services.Dialogs.Base;

namespace RtspViewer.GUI.Services.Dialogs
{
    public interface IFolderDialogService : IDialogService
    {
        string SelectedFolder { get; }
        bool SelectFolderDialog();
    }
}
