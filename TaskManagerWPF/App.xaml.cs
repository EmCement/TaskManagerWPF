using System.Windows;
using TaskManagerWPF.Services;

namespace TaskManagerWPF
{
    public partial class App : Application
    {
        public static ApiService ApiService { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            ApiService = new ApiService("http://fin.gitbebra.ru:24578/");
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}