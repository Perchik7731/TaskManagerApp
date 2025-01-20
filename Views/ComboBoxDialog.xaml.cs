using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TaskManagerApp.Views
{
    public partial class ComboBoxDialog : Window
    {
        public string SelectedItem { get; private set; }

        public ComboBoxDialog(string title, List<string> items)
        {
            InitializeComponent();
            TitleLabel.Content = title;
            ItemsComboBox.ItemsSource = items;
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsComboBox.SelectedItem is string selected)
            {
                SelectedItem = selected;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}