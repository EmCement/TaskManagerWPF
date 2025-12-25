using System.Windows;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;
        private readonly User _currentUser;

        public MainWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            _navigationService = new NavigationService(this);

            Title = $"Менеджер задач - {currentUser.Username}";
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToDashboard(_currentUser, this);
        }

        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToProjects(_currentUser, this);
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToTasks(_currentUser, this);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.ShowProfileDialog(_currentUser, this);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.Logout(this);
        }
    }
}