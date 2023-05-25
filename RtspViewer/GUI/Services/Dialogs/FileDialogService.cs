using System.Windows.Forms;
using RtspViewer.GUI.Services.Dialogs.Base;

namespace RtspViewer.GUI.Services.Dialogs
{
    public class FileDialogService : DialogService, IFileDialogService
    {
        public string SelectedFile { get; private set; } = "";
        public bool SelectFileDialog(string defaultExt = "")
        {
            using var dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = defaultExt
            };
            dialog.ShowDialog();
            SelectedFile = dialog.FileName;
            return SelectedFile != "";
        }
    }
}
