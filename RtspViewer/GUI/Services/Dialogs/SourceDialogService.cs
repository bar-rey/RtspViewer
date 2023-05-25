using RtspViewer.GUI.DialogBoxes;
using RtspViewer.GUI.Models;
using RtspViewer.GUI.Services.Dialogs.Base;

namespace RtspViewer.GUI.Services.Dialogs
{
    public class SourceDialogService : DialogService, ISourceDialogService
    {
        public Source? ModifiedSource { get; private set; }

        public bool ModifySourceDialog(Source source)
        {
            var dialog = new ModifySourceDialog(source);
            if (dialog.ShowDialog() == true)
            {
                ModifiedSource = dialog.Source;
                return true;
            }
            return false;
        }
    }
}
