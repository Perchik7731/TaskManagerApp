using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class AddTaskWindow : Window
    {
        private DatabaseService _databaseService;

        public AddTaskWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            LoadProjects();
            LoadUsers();
            App.ThemeChanged += App_ThemeChanged;
        }
        private void App_ThemeChanged(object sender, EventArgs e)
        {
            Resources.MergedDictionaries.Clear();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new Models.Task
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                Priority = ((ComboBoxItem)PriorityComboBox.SelectedItem)?.Content.ToString(),
                DueDate = DueDatePicker.SelectedDate.HasValue ? DueDatePicker.SelectedDate.Value : (DateTime?)null,
                Status = ((ComboBoxItem)StatusComboBox.SelectedItem)?.Content.ToString(),
                ProjectId = (ProjectComboBox.SelectedItem as Project)?.Id,
                AssigneeId = (AssigneeComboBox.SelectedItem as User)?.Id
            };

            _databaseService.CreateTask(task);
            Close();
        }
        private void LoadProjects()
        {
            List<Project> projects = _databaseService.GetAllProjects();
            ProjectComboBox.ItemsSource = projects;
        }
        private void LoadUsers()
        {
            List<User> users = _databaseService.GetAllUsers();
            AssigneeComboBox.ItemsSource = users;
        }

    }
}