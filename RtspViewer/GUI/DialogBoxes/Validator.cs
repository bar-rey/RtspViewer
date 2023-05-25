using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace RtspViewer.GUI.DialogBoxes
{
    /// <summary>
    /// Валидация свойств зависимостей
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Валидация свойства зависимостей
        /// </summary>
        /// <param name="obj">Свойство зависимостей, которое будет валидироваться</param>
        /// <returns>Нету ли ошибок валидации</returns>
        public static bool IsValid(DependencyObject obj)
        {
            return !Validation.GetHasError(obj) &&
                LogicalTreeHelper.GetChildren(obj)
                .OfType<DependencyObject>()
                .All(IsValid);
        }
    }
}
