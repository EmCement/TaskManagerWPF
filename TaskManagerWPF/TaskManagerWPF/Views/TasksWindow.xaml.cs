using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class TasksWindow : Window
    {
        private readonly User _currentUser;
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        // Исправлено: должно быть ObservableCollection<TaskDisplayItem>
        private ObservableCollection<TaskDisplayItem> _tasks = new ObservableCollection<TaskDisplayItem>();

        // Модель для отображения
        public class TaskDisplayItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string ProjectName { get; set; } = "Без проекта";
            public string PriorityName { get; set; } = "Не задан";
            public string StatusName { get; set; } = "Не задан";
            public string AssigneeNames { get; set; } = "Не назначен";
            public DateTime? DueDate { get; set; }
            public string DueDateDisplay => DueDate?.ToString("dd.MM.yyyy") ?? "Нет срока";
            public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now;
        }

        public TasksWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            // Исправлено: правильное получение ApiService
            _apiService = App.ApiService;
            _navigationService = new NavigationService(this);

            Title = $"Задачи - {currentUser.Username}";
            TasksDataGrid.ItemsSource = _tasks;

            LoadTasksAsync(); // Вызов метода
        }

        // Метод должен быть private async void (для обработчиков событий)
        private async void LoadTasksAsync()
        {
            try
            {
                // Очищаем список
                _tasks.Clear();

                // Загружаем задачи с API
                var apiTasks = await _apiService.GetTasksAsync();

                // Конвертируем в модель для отображения
                foreach (var task in apiTasks)
                {
                    var displayItem = new TaskDisplayItem
                    {
                        Id = task.Id,
                        Title = task.Title,
                        DueDate = task.DueDate,
                        ProjectName = task.Project?.Name ?? "Без проекта",
                        PriorityName = task.Priority?.Name ?? "Не задан",
                        StatusName = task.Status?.Name ?? "Не задан",
                        AssigneeNames = "Не назначен" // Заглушка
                    };

                    _tasks.Add(displayItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки задач: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.ShowTaskDetailsDialog(null, this);
            LoadTasksAsync(); // перезагружаем после создания
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Фильтрация будет добавлена в следующем обновлении",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int taskId)
            {
                _navigationService.ShowTaskDetailsDialog(taskId, this);
                LoadTasksAsync(); // перезагружаем после редактирования
            }
        }

        private async void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int taskId)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить задачу #{taskId}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteTaskAsync(taskId);

                        var taskToRemove = _tasks.FirstOrDefault(t => t.Id == taskId);
                        if (taskToRemove != null)
                        {
                            _tasks.Remove(taskToRemove);
                        }

                        MessageBox.Show("Задача успешно удалена",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении задачи: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void TasksDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is TaskDisplayItem selectedTask)
            {
                _navigationService.ShowTaskDetailsDialog(selectedTask.Id, this);
                LoadTasksAsync();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                LoadTasksAsync();
            }
        }
    }
}