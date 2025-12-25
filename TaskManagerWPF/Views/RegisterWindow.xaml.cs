using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => UsernameTextBox.Focus();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string fullName = FullNameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (!ValidateInputs(username, email, password, confirmPassword))
                return;

            RegisterButton.IsEnabled = false;
            RegisterButton.Content = "Регистрация...";

            try
            {
                var registerData = new UserRegister
                {
                    Username = username,
                    Email = email,
                    Password = password,
                    FullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName
                };

                var user = await App.ApiService.RegisterAsync(registerData);

                MessageBox.Show($"Пользователь {user.Username} успешно зарегистрирован!\nТеперь выполните вход.",
                    "Успешная регистрация", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}",
                    "Ошибка сети", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                RegisterButton.Content = "Зарегистрироваться";
            }
        }

        private bool ValidateInputs(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return false;
            }

            if (username.Length < 3)
            {
                MessageBox.Show("Имя пользователя должно содержать минимум 3 символа", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Введите email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email адрес", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return false;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return false;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ConfirmPasswordBox.Focus();
                ConfirmPasswordBox.SelectAll();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UsernameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                EmailTextBox.Focus();
        }

        private void EmailTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                FullNameTextBox.Focus();
        }

        private void FullNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                PasswordBox.Focus();
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                ConfirmPasswordBox.Focus();
        }

        private void ConfirmPasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                RegisterButton_Click(sender, e);
        }
    }
}