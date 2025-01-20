using System;
using System.Collections.Generic;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Services;

namespace TaskManagerApp.Views
{
    public partial class TaskCommentsWindow : Window
    {
        private DatabaseService _databaseService;
        private Models.Task _task;
        private NotificationService _notificationService;

        public TaskCommentsWindow(DatabaseService databaseService, Models.Task task)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _task = task;
            LoadComments();
        }

        private void LoadComments()
        {
            List<Comment> comments = _databaseService.GetCommentsByTaskId(_task.Id);
            foreach (var comment in comments)
            {
                comment.User = _databaseService.GetUserById(comment.UserId);
            }
            CommentsListView.ItemsSource = comments;
        }

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите текст комментария.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var comment = new Comment
            {
                TaskId = _task.Id,
                UserId = MainWindow.CurrentUser.Id, 
                Text = CommentTextBox.Text,
                CreatedAt = DateTime.Now
            };

            _databaseService.CreateComment(comment);
            CommentTextBox.Clear();
            _notificationService.CheckForNewComments();
            LoadComments();
        }
    }
}