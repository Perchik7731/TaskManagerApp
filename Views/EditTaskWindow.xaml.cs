using Dapper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Services;

namespace TaskManagerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        private DatabaseService _databaseService;
        private Models.Task _task;
        private NotificationService _notificationService;
        public EditTaskWindow(DatabaseService databaseService, Models.Task task)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _task = task;
            LoadTaskDetails();
            LoadProjects();
            LoadUsers();
            LoadTeams();
        }

        private void LoadTaskDetails()
        {
            TitleTextBox.Text = _task.Title;
            DescriptionTextBox.Text = _task.Description;
            if (_task.Priority != null)
            {
                foreach (ComboBoxItem item in PriorityComboBox.Items)
                {
                    if (item.Content.ToString() == _task.Priority)
                    {
                        PriorityComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            DueDatePicker.SelectedDate = _task.DueDate;
            if (_task.Status != null)
            {
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString() == _task.Status)
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (_task.ProjectId.HasValue)
                ProjectComboBox.SelectedItem = _databaseService.GetProjectById(_task.ProjectId.Value);

            if (_task.AssigneeId.HasValue)
                AssigneeComboBox.SelectedItem = _databaseService.GetUserById(_task.AssigneeId.Value);

            if (_task.TeamId.HasValue)
                TeamComboBox.SelectedItem = _databaseService.GetTeamById(_task.TeamId.Value);
        }


        private void LoadTeams()
        {
            List<Team> teams = _databaseService.GetAllTeams();
            TeamComboBox.ItemsSource = teams;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _task.Title = TitleTextBox.Text;
            _task.Description = DescriptionTextBox.Text;
            _task.Priority = ((ComboBoxItem)PriorityComboBox.SelectedItem)?.Content.ToString();
            _task.DueDate = DueDatePicker.SelectedDate.HasValue ? DueDatePicker.SelectedDate.Value : (DateTime?)null;
            _task.Status = ((ComboBoxItem)StatusComboBox.SelectedItem)?.Content.ToString();
            _task.ProjectId = (ProjectComboBox.SelectedItem as Project)?.Id;
            _task.AssigneeId = (AssigneeComboBox.SelectedItem as User)?.Id;

            var oldTeamId = _task.TeamId;
            _task.TeamId = (TeamComboBox.SelectedItem as Team)?.Id;

            _databaseService.UpdateTask(_task);

            if (oldTeamId != _task.TeamId && _task.TeamId.HasValue)
                _databaseService.AddTaskToTeam(_task.Id, _task.TeamId.Value);
            else if (!_task.TeamId.HasValue && oldTeamId.HasValue)
            {
                using (var connection = _databaseService.GetConnection())
                {
                    connection.Execute("DELETE FROM TaskTeams WHERE TaskId = @Id", new { Id = _task.Id });
                }
            }
            if (_task.Id > 0)
                _task.Team = _databaseService.GetTaskTeam(_task.Id);
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
