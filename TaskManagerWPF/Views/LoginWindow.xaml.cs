using System;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => UsernameTextBox.Focus();

            CheckSavedLogin();
        }

        private void CheckSavedLogin()
        {
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoginButton.IsEnabled = false;
            LoginButton.Content = "Вход...";

            try
            {
                var tokenResponse = await App.ApiService.LoginAsync(username, password);

                App.ApiService.SetToken(tokenResponse.AccessToken);

                var currentUser = await App.ApiService.GetCurrentUserAsync();

                OpenMainWindow(currentUser);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("Неверное имя пользователя или пароль",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}",
                    "Ошибка сети", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Войти";
            }
        }

        private void OpenMainWindow(User currentUser)
        {
            var mainWindow = new MainWindow(currentUser);
            mainWindow.Show();
            this.Close();
        }

        private void UsernameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
    }
}