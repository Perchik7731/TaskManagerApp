using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using Task = TaskManagerApp.Models.Task;
using TaskManagerApp.Views;
using TaskManagerApp.Services;
using System.IO;

namespace TaskManagerApp
{
    public partial class MainWindow : Window
    {
        private DatabaseService _databaseService;
        private NotificationService _notificationService;
        public static User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService(_databaseService);
            LoadTasks();
            this.ContentRendered += MainWindow_ContentRendered;
            App.ThemeChanged += App_ThemeChanged;
        }
        private void App_ThemeChanged(object sender, EventArgs e)
        {
            var app = (App)Application.Current;
            Resources.MergedDictionaries.Clear();
            if (File.Exists("settings.xml"))
            {
                try
                {
                    System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load("settings.xml");
                    System.Xml.Linq.XElement root = doc.Root;
                    if (root != null)
                    {
                        var themeElement = root.Element("Theme");
                        if (themeElement != null)
                        {
                            string theme = themeElement.Value;
                            if (theme == "Светлая")
                                Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) });
                            else if (theme == "Темная")
                                Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) });
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) });
            }
        }


        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            _notificationService.CheckForDueTasks();
            _notificationService.CheckForAssignedTasks();
            _notificationService.CheckForNewComments();
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow(_databaseService);
            addTaskWindow.ShowDialog();
            LoadTasks();
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is Task selectedTask)
            {
                var editTaskWindow = new EditTaskWindow(_databaseService, selectedTask);
                editTaskWindow.ShowDialog();
                LoadTasks();
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is Task selectedTask)
            {
                _databaseService.DeleteTask(selectedTask.Id);
                LoadTasks();
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var addProjectWindow = new AddProjectWindow(_databaseService);
            addProjectWindow.ShowDialog();
            LoadTasks();
        }
        private void ViewCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is Task selectedTask)
            {
                var taskCommentsWindow = new TaskCommentsWindow(_databaseService, selectedTask);
                taskCommentsWindow.ShowDialog();
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTasks();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadTasks();
        }
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTasks();
        }
        private void LoadTasks()
        {
            var tasks = _databaseService.GetAllTasks();

            foreach (var task in tasks)
            {
                if (task.ProjectId.HasValue)
                    task.Project = _databaseService.GetProjectById(task.ProjectId.Value);

                if (task.AssigneeId.HasValue)
                    task.Assignee = _databaseService.GetUserById(task.AssigneeId.Value);
            }

            var filteredTasks = FilterTasks(tasks);
            var searchedTasks = SearchTasks(filteredTasks);
            var sortedTasks = SortTasks(searchedTasks);
            TasksDataGrid.ItemsSource = sortedTasks;
        }

        private List<Task> FilterTasks(List<Task> tasks)
        {
            if (FilterComboBox.SelectedItem is ComboBoxItem selectedFilter)
            {
                string filterType = selectedFilter.Content.ToString();

                switch (filterType)
                {
                    case "Статус":
                        var statuses = tasks.Select(t => t.Status).Distinct().ToList();
                        var selectedStatus = ShowComboBoxDialog("Выберите статус", statuses);
                        if (string.IsNullOrEmpty(selectedStatus))
                            return tasks;
                        return tasks.Where(task => task.Status == selectedStatus).ToList();
                    case "Приоритет":
                        var priorities = tasks.Select(t => t.Priority).Distinct().ToList();
                        var selectedPriority = ShowComboBoxDialog("Выберите приоритет", priorities);
                        if (string.IsNullOrEmpty(selectedPriority))
                            return tasks;
                        return tasks.Where(task => task.Priority == selectedPriority).ToList();
                    case "Проект":
                        var projects = _databaseService.GetAllProjects().Select(p => p.Title).ToList();
                        var selectedProject = ShowComboBoxDialog("Выберите проект", projects);
                        if (string.IsNullOrEmpty(selectedProject))
                            return tasks;
                        return tasks.Where(task => task.Project != null && task.Project.Title == selectedProject).ToList();
                    case "Исполнитель":
                        var assignees = _databaseService.GetAllUsers().Select(u => u.Login).ToList();
                        var selectedAssignee = ShowComboBoxDialog("Выберите исполнителя", assignees);
                        if (string.IsNullOrEmpty(selectedAssignee))
                            return tasks;
                        return tasks.Where(task => task.Assignee != null && task.Assignee.Login == selectedAssignee).ToList();
                    default:
                        return tasks;
                }
            }
            return tasks;
        }
        private string ShowComboBoxDialog(string title, List<string> items)
        {
            var dialog = new ComboBoxDialog(title, items);
            dialog.ShowDialog();
            return dialog.SelectedItem;
        }

        private List<Task> SearchTasks(List<Task> tasks)
        {
            var searchText = SearchTextBox.Text;
            if (string.IsNullOrEmpty(searchText))
                return tasks;
            return tasks.Where(task =>
            (!string.IsNullOrEmpty(task.Title) && task.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(task.Description) && task.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        private List<Task> SortTasks(List<Task> tasks)
        {
            if (SortComboBox.SelectedItem is ComboBoxItem selectedSort)
            {
                string sortType = selectedSort.Content.ToString();
                switch (sortType)
                {
                    case "Дате":
                        return tasks.OrderBy(task => task.DueDate).ToList();
                    case "Приоритету":
                        return tasks.OrderBy(task => task.Priority).ToList();
                    default:
                        return tasks;
                }
            }
            return tasks;
        }

        private void ViewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var userProfileWindow = new UserProfileWindow(_databaseService, CurrentUser);
            userProfileWindow.ShowDialog();
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is Task selectedTask && selectedTask.Project != null)
            {
                var editProjectWindow = new EditProjectWindow(_databaseService, selectedTask.Project);
                editProjectWindow.ShowDialog();
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Выберите задачу связанную с проектом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is Task selectedTask && selectedTask.Project != null)
            {
                _databaseService.DeleteProject(selectedTask.Project.Id);
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Выберите задачу связанную с проектом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ViewTeamManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var teamManagementWindow = new TeamManagementWindow(_databaseService);
            teamManagementWindow.ShowDialog();
        }
        private void ViewUserManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser != null)
            {
                
                List<string> roles = _databaseService.GetUserRoles(CurrentUser.Id);

                
                if (roles != null && roles.Contains("Администратор"))
                {
                    
                    var userManagementWindow = new UserManagementWindow(_databaseService);
                    List<User> users = _databaseService.GetAllUsers();
                    foreach (var user in users)
                    {
                        user.Roles = _databaseService.GetUserRoles(user.Id);
                    }
                    userManagementWindow.UsersDataGrid.ItemsSource = users;
                    userManagementWindow.ShowDialog();
                }
                else
                {
                    
                    MessageBox.Show("У вас нет прав для доступа к управлению пользователями.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void ViewSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}