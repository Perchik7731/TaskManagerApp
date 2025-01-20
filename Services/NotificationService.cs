using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Services
{
    public class NotificationService
    {
        private readonly DatabaseService _databaseService;
        private List<int> _notifiedTaskIds = new List<int>();


        public NotificationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public void CheckForDueTasks()
        {
            List<Models.Task> tasks = _databaseService.GetAllTasks();
            foreach (var task in tasks)
            {
                if (task.DueDate.HasValue)
                {
                    TimeSpan timeLeft = task.DueDate.Value - DateTime.Now;
                    if (timeLeft.TotalDays <= 3 && timeLeft.TotalDays > 0) //Уведомление за 3 дня
                    {
                        ShowNotification(task, $"Задача '{task.Title}' истекает через {timeLeft.Days} дней.");
                    }
                    else if (timeLeft.TotalDays < 0 && task.Status != "Выполнена")
                    {
                        ShowNotification(task, $"Задача '{task.Title}' просрочена.");
                    }
                }
            }
        }
        public void CheckForAssignedTasks()
        {
            List<Models.Task> tasks = _databaseService.GetAllTasks();
            foreach (var task in tasks)
            {
                if (task.AssigneeId.HasValue && !_notifiedTaskIds.Contains(task.Id))
                {
                    var assignee = _databaseService.GetUserById(task.AssigneeId.Value);
                    if (assignee != null)
                    {
                        ShowNotification(task, $"Вы назначены исполнителем задачи '{task.Title}'");
                        _notifiedTaskIds.Add(task.Id);
                    }
                }
            }

        }

        public void CheckForNewComments()
        {
            List<Comment> comments = _databaseService.GetAllComments();
            foreach (var comment in comments)
            {
                var task = _databaseService.GetTaskById(comment.TaskId);
                if (task != null && task.AssigneeId.HasValue)
                {
                    var assignee = _databaseService.GetUserById(task.AssigneeId.Value);
                    if (assignee != null)
                    {
                        ShowNotification(task, $"К вашей задаче '{task.Title}' добавлен новый комментарий.");
                    }

                }
            }
        }
        private void ShowNotification(Models.Task task, string message)
        {
            MessageBox.Show(message, "Уведомление о задаче", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}