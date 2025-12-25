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
    public partial class ProjectsWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        // Исправлено: объявляем _projects
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();

        public ProjectsWindow()
        {
            InitializeComponent();

            _apiService = App.ApiService;
            _navigationService = new NavigationService(this);

            // Настройка DataGrid
            ProjectsGrid.ItemsSource = _projects;

            // Загрузка данных
            LoadProjectsAsync(); // Вызов метода
        }

        // Исправлено: метод должен быть private async void
        private async void LoadProjectsAsync()
        {
            try
            {
                ProjectsGrid.ItemsSource = null;

                var projects = await _apiService.GetProjectsAsync();

                _projects.Clear();
                foreach (var project in projects)
                {
                    _projects.Add(project);
                }

                ProjectsGrid.ItemsSource = _projects;

                // Статус в заголовке
                Title = $"Проекты ({_projects.Count}) - Менеджер задач";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки проектов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.ShowProjectCreateDialog(this);
            LoadProjectsAsync(); // перезагружаем после создания
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int projectId)
            {
                var project = _projects.FirstOrDefault(p => p.Id == projectId);
                if (project != null)
                {
                    // TODO: Создать окно редактирования проекта
                    MessageBox.Show($"Редактирование проекта #{projectId} будет в следующем обновлении",
                        "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int projectId)
            {
                var project = _projects.FirstOrDefault(p => p.Id == projectId);
                if (project == null) return;

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить проект '{project.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteProjectAsync(projectId);

                        _projects.Remove(project);

                        MessageBox.Show($"Проект '{project.Name}' удален",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProjectsAsync();
        }

        private void ProjectsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ProjectsGrid.SelectedItem is Project selectedProject)
            {
                // TODO: Открыть окно с задачами этого проекта
                MessageBox.Show($"Задачи проекта '{selectedProject.Name}' будут показаны в следующем обновлении",
                    "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                LoadProjectsAsync();
            }
        }
    }
}