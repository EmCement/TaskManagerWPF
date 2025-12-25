using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        public class TaskDisplayItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public TaskWithDetails Task { get; set; } = null!;
            public string ProjectName => Task.Project?.Name ?? "Без проекта";
            public DateTime? DueDate => Task.DueDate;
            public string DueDateDisplay => Task.DueDate.HasValue
                ? $"Срок: {Task.DueDate.Value:dd.MM.yyyy}"
                : "Срок не указан";
            public string StatusName => Task.Status?.Name ?? "Без статуса";
            public string PriorityName => Task.Priority?.Name ?? "Без приоритета";
            public string Description => Task.Description ?? "";
            public bool IsOverdue => Task.DueDate.HasValue &&
                                    Task.DueDate.Value < DateTime.Now &&
                                    (Task.Status == null || !Task.Status.IsFinal);
            public bool IsCompleted => Task.Status != null && Task.Status.IsFinal;
        }

        private ObservableCollection<TaskDisplayItem> _recentTasks = new ObservableCollection<TaskDisplayItem>();

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            _apiService = App.ApiService;
            _navigationService = new NavigationService(this);

            WelcomeText.Text = $"Добро пожаловать, {currentUser.Username}!";

            LoadDashboardData();

            RecentTasksList.ItemsSource = _recentTasks;
        }

        private async void LoadDashboardData()
        {
            try
            {
                var projectsTask = _apiService.GetProjectsAsync();
                var tasksTask = _apiService.GetTasksAsync();

                await Task.WhenAll(projectsTask, tasksTask);

                var projects = await projectsTask;
                var tasks = await tasksTask;

                UpdateStatistics(projects, tasks);
                UpdateRecentTasks(tasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics(List<Project> projects, List<TaskWithDetails> tasks)
        {
            TotalProjectsText.Text = projects.Count.ToString();

            var activeTasks = tasks.Count(t =>
                t.Status == null || !t.Status.IsFinal);
            ActiveTasksText.Text = activeTasks.ToString();

            var overdueTasks = tasks.Count(t =>
                (t.Status == null || !t.Status.IsFinal) &&
                t.DueDate.HasValue &&
                t.DueDate.Value < DateTime.Now);
            OverdueTasksText.Text = overdueTasks.ToString();

            var completedTasks = tasks.Count(t =>
                t.Status != null && t.Status.IsFinal);
            CompletedTasksText.Text = completedTasks.ToString();
        }

        private void UpdateRecentTasks(System.Collections.Generic.List<TaskWithDetails> tasks)
        {
            var recent = tasks
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new TaskDisplayItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    Task = t
                });

            _recentTasks.Clear();
            foreach (var task in recent)
            {
                _recentTasks.Add(task);
            }
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.ShowProjectCreateDialog(this);
            LoadDashboardData();
        }

        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.ShowTaskDetailsDialog(null, this);
            LoadDashboardData();
        }

        private void AllTasks_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToTasks(_currentUser, this);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.ShowProfileDialog(_currentUser, this);
        }

        private void AllProjects_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToProjects(_currentUser, this);
        }

        private void RecentTasksList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (RecentTasksList.SelectedItem is TaskDisplayItem selectedTask)
            {
                _navigationService.ShowTaskDetailsDialog(selectedTask.Id, this);
                LoadDashboardData();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }
    }
}
