using System;
using System.Windows;
using TaskManagerWPF.Models;
using TaskManagerWPF.Views;

namespace TaskManagerWPF.Services
{
    public class NavigationService
    {
        private Window _currentWindow;

        public NavigationService(Window initialWindow)
        {
            _currentWindow = initialWindow;
        }

        public void NavigateToDashboard(User user, Window owner)
        {
            var dashboardWindow = new DashboardWindow(user);
            dashboardWindow.Owner = owner;
            dashboardWindow.ShowDialog();
        }

        public void NavigateToProjects(User user, Window owner)
        {
            var projectsWindow = new ProjectsWindow();
            projectsWindow.Owner = owner;
            projectsWindow.ShowDialog();
        }

        public void NavigateToTasks(User user, Window owner)
        {
            var tasksWindow = new TasksWindow(user);
            tasksWindow.Owner = owner;
            tasksWindow.ShowDialog();
        }

        public void ShowProfileDialog(User user, Window owner)
        {
            var profileWindow = new ProfileWindow(user);
            profileWindow.Owner = owner;
            profileWindow.ShowDialog();
        }

        public void ShowTaskDetailsDialog(int? taskId = null, Window owner = null)
        {
            var taskWindow = taskId.HasValue
                ? new TaskDetailsWindow(taskId.Value)
                : new TaskDetailsWindow();

            if (owner != null)
                taskWindow.Owner = owner;

            taskWindow.ShowDialog();
        }

        public void ShowProjectCreateDialog(Window owner = null)
        {
            var projectWindow = new ProjectCreateWindow();

            if (owner != null)
                projectWindow.Owner = owner;

            projectWindow.ShowDialog();
        }

        public void Logout(Window currentWindow)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var app = (App)Application.Current;
                App.ApiService.ClearToken();

                var loginWindow = new LoginWindow();
                loginWindow.Show();

                currentWindow.Close();
            }
        }

        private void NavigateToWindow(Window newWindow)
        {
            newWindow.Left = _currentWindow.Left;
            newWindow.Top = _currentWindow.Top;
            newWindow.Width = _currentWindow.Width;
            newWindow.Height = _currentWindow.Height;
            newWindow.WindowState = _currentWindow.WindowState;

            newWindow.Show();
            _currentWindow.Close();
            _currentWindow = newWindow;
        }
    }
}