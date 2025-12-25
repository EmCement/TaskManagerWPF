using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class TaskDetailsWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly int? _taskId;
        private List<Project> _projects = new();
        private List<Priority> _priorities = new();
        private List<Status> _statuses = new();
        private List<User> _users = new();

        public TaskDetailsWindow()
        {
            InitializeComponent();

            _apiService = App.ApiService;
            _taskId = null;

            InitializeWindow();
            LoadDataAsync();
        }

        public TaskDetailsWindow(int taskId)
        {
            InitializeComponent();

            _apiService = App.ApiService;
            _taskId = taskId;

            InitializeWindow();
            LoadDataAsync();
        }

        private void InitializeWindow()
        {
            if (_taskId.HasValue)
            {
                TitleText.Text = "Редактирование задачи";
                Title = $"Редактирование задачи #{_taskId}";
                SaveButton.Content = "Обновить";
            }
            else
            {
                TitleText.Text = "Новая задача";
                Title = "Новая задача";
                SaveButton.Content = "Создать";
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var projectsTask = _apiService.GetProjectsAsync();
                var prioritiesTask = _apiService.GetPrioritiesAsync();
                var statusesTask = _apiService.GetStatusesAsync();

                await Task.WhenAll(projectsTask, prioritiesTask, statusesTask);

                _projects = await projectsTask;
                _priorities = await prioritiesTask;
                _statuses = await statusesTask;
                // _users = await usersTask;

                FillComboBoxes();

                if (_taskId.HasValue)
                {
                    await LoadTaskDataAsync(_taskId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillComboBoxes()
        {
            ProjectComboBox.Items.Clear();
            foreach (var project in _projects)
            {
                ProjectComboBox.Items.Add(new ComboBoxItem
                {
                    Content = project.Name,
                    Tag = project.Id
                });
            }
            ProjectComboBox.SelectedIndex = 0;

            PriorityComboBox.Items.Clear();
            PriorityComboBox.Items.Add(new ComboBoxItem
            {
                Content = "Не задан",
                Tag = (int?)null
            });
            foreach (var priority in _priorities.OrderBy(p => p.Level))
            {
                PriorityComboBox.Items.Add(new ComboBoxItem
                {
                    Content = priority.Name,
                    Tag = priority.Id
                });
            }
            PriorityComboBox.SelectedIndex = 0;

            StatusComboBox.Items.Clear();
            StatusComboBox.Items.Add(new ComboBoxItem
            {
                Content = "Не задан",
                Tag = (int?)null
            });
            foreach (var status in _statuses.OrderBy(s => s.OrderNum))
            {
                StatusComboBox.Items.Add(new ComboBoxItem
                {
                    Content = status.Name,
                    Tag = status.Id
                });
            }
            StatusComboBox.SelectedIndex = 0;
        }

        private async Task LoadTaskDataAsync(int taskId)
        {
            try
            {
                var task = await _apiService.GetTaskAsync(taskId);

                TitleTextBox.Text = task.Title;
                DescriptionTextBox.Text = task.Description ?? "";
                DueDatePicker.SelectedDate = task.DueDate;

                foreach (ComboBoxItem item in ProjectComboBox.Items)
                {
                    if (item.Tag is int projectId && projectId == task.ProjectId)
                    {
                        ProjectComboBox.SelectedItem = item;
                        break;
                    }
                }

                if (task.PriorityId.HasValue)
                {
                    foreach (ComboBoxItem item in PriorityComboBox.Items)
                    {
                        if (item.Tag is int priorityId && priorityId == task.PriorityId.Value)
                        {
                            PriorityComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (task.StatusId.HasValue)
                {
                    foreach (ComboBoxItem item in StatusComboBox.Items)
                    {
                        if (item.Tag is int statusId && statusId == task.StatusId.Value)
                        {
                            StatusComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки задачи: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название задачи",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleTextBox.Focus();
                return;
            }

            if (ProjectComboBox.SelectedItem is not ComboBoxItem selectedProject)
            {
                MessageBox.Show("Выберите проект",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var taskData = new TaskCreate
                {
                    Title = TitleTextBox.Text.Trim(),
                    Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
                        ? null
                        : DescriptionTextBox.Text.Trim(),
                    ProjectId = (int)selectedProject.Tag!,
                    DueDate = DueDatePicker.SelectedDate
                };

                if (PriorityComboBox.SelectedItem is ComboBoxItem selectedPriority &&
                    selectedPriority.Tag is int priorityId)
                {
                    taskData.PriorityId = priorityId;
                }

                if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatus &&
                    selectedStatus.Tag is int statusId)
                {
                    taskData.StatusId = statusId;
                }

                SaveButton.IsEnabled = false;
                SaveButton.Content = "Сохранение...";

                if (_taskId.HasValue)
                {
                    await _apiService.UpdateTaskAsync(_taskId.Value, taskData);
                    MessageBox.Show("Задача успешно обновлена",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _apiService.CreateTaskAsync(taskData);
                    MessageBox.Show("Задача успешно создана",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = _taskId.HasValue ? "Обновить" : "Создать";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(TitleTextBox.Text);
        }
    }
}