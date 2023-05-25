using RtspViewer.GUI.Models;
using RtspViewer.GUI.Services.Dialogs.Base;

namespace RtspViewer.GUI.Services.Dialogs
{
    public interface ISourceDialogService : IDialogService
    {
        public Source? ModifiedSource { get; }
        public bool ModifySourceDialog(Source source);
    }
}
