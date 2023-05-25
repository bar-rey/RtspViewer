namespace RtspViewer.GUI.Services.Dialogs.Base
{
    public interface IDialogService
    {
        /// <summary>
        /// Диалоговое окно с сообщением
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        void ShowMessage(string message);
        /// <summary>
        /// Диалоговое окно с сообщением об ошибке
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        void ShowErrorMessage(string message);
        /// <summary>
        /// Диалоговое окно с информационным сообщением
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        void ShowInformationMessage(string message);
        /// <summary>
        /// Диалоговое окно для подтверждающим сообщением
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <returns></returns>
        bool ShowConfirmationMessage(string message);
    }
}
