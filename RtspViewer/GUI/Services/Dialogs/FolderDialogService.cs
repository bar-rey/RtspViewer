using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RtspViewer.GUI.Services.Dialogs.Base;

namespace RtspViewer.GUI.Services.Dialogs
{
    public class FolderDialogService : DialogService, IFolderDialogService
    {
        public string SelectedFolder { get; private set; } = "";

        public bool SelectFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            SelectedFolder = dialog.SelectedPath;
            return SelectedFolder != "";
        }
    }
}
