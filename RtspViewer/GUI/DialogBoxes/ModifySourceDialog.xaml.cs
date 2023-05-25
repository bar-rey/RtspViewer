using System.Windows;
using RtspViewer.GUI.Models;

namespace RtspViewer.GUI.DialogBoxes
{
    /// <summary>
    /// Логика взаимодействия для ModifySourceDialog.xaml
    /// </summary>
    public partial class ModifySourceDialog : Window
    {
        public Source Source { get; private set; }
        public ModifySourceDialog(Source source)
        {
            InitializeComponent();
            Source = source;
            DataContext = source;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (Validator.IsValid(this))
                DialogResult = true;
            else
                MessageBox.Show("Имеются ошибки валидации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
