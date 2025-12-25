using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TaskManagerWPF.Models;
using TaskManagerWPF.Services;

namespace TaskManagerWPF.Views
{
    public partial class ProjectCreateWindow : Window
    {
        private readonly ApiService _apiService;

        public ProjectCreateWindow()
        {
            InitializeComponent();

            _apiService = App.ApiService;

            Loaded += (s, e) => NameTextBox.Focus();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название проекта",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            var projectData = new ProjectCreate
            {
                Name = NameTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
                    ? null
                    : DescriptionTextBox.Text.Trim()
            };

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Сохранение...";

                var createdProject = await _apiService.CreateProjectAsync(projectData);

                MessageBox.Show($"Проект '{createdProject.Name}' успешно создан!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании проекта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                SaveButton.IsEnabled = true;
                SaveButton.Content = "Сохранить";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                DescriptionTextBox.Focus();
            }
        }

        private void DescriptionTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter &&
                (Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                Save_Click(sender, e);
            }
        }

        private void NameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(NameTextBox.Text);
        }
    }
}