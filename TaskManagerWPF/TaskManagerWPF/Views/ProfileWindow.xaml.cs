using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class ProfileWindow : Window
    {
        private readonly ApiService _apiService;
        private User _currentUser;

        public ProfileWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            _apiService = App.ApiService;

            LoadUserData();
            LoadUserStatisticsAsync();
        }

        private void LoadUserData()
        {
            // Устанавливаем данные пользователя
            UserNameText.Text = _currentUser.FullName ?? _currentUser.Username;
            LoginText.Text = _currentUser.Username;
            EmailText.Text = _currentUser.Email;

            // Аватар (первые буквы имени)
            if (!string.IsNullOrEmpty(_currentUser.FullName))
            {
                var names = _currentUser.FullName.Split(' ');
                AvatarText.Text = names.Length > 1
                    ? $"{names[0][0]}{names[1][0]}".ToUpper()
                    : _currentUser.FullName[0].ToString().ToUpper();
            }
            else
            {
                AvatarText.Text = _currentUser.Username[0].ToString().ToUpper();
            }

            // Роль
            RoleText.Text = _currentUser.Role switch
            {
                "admin" => "Администратор",
                "user" => "Пользователь",
                _ => _currentUser.Role
            };

            // Дата регистрации
            RegisterDateText.Text = _currentUser.CreatedAt.ToString("dd.MM.yyyy");
        }

        private async void LoadUserStatisticsAsync()
        {
            try
            {
                var tasks = await _apiService.GetTasksAsync();

                // Фильтруем задачи пользователя (если API не поддерживает фильтр по пользователю)
                var userTasks = tasks
                    .Where(t => t.CreatedById == _currentUser.Id)
                    .ToList();

                // Активные задачи (не завершенные)
                var activeTasks = userTasks.Count(t =>
                    t.Status == null || !t.Status.IsFinal);
                ActiveTasksText.Text = activeTasks.ToString();

                // Завершенные задачи
                var completedTasks = userTasks.Count(t =>
                    t.Status != null && t.Status.IsFinal);
                CompletedTasksText.Text = completedTasks.ToString();

                // Просроченные задачи
                var overdueTasks = userTasks.Count(t =>
                    t.DueDate.HasValue && t.DueDate.Value < DateTime.Now &&
                    (t.Status == null || !t.Status.IsFinal));
                OverdueTasksText.Text = overdueTasks.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование профиля будет в следующем обновлении",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Смена пароля будет в следующем обновлении",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обновление данных при открытии окна
        protected override async void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // Обновляем данные пользователя (на случай если изменились)
            try
            {
                _currentUser = await _apiService.GetCurrentUserAsync();
                LoadUserData();
                LoadUserStatisticsAsync();
            }
            catch
            {
                // Игнорируем ошибки при обновлении
            }
        }
    }
}