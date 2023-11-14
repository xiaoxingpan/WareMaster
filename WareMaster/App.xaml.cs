using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WareMaster
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create and show the login window
            Login loginWindow = new Login();
            loginWindow.ShowDialog();

            if (loginWindow.IsAuthenticated)
            {
                // If the user is authenticated, create and show the main window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

            }
            else
            {
                // If authentication fails or the user cancels the login, you can handle it here.
                // For example, you can exit the application or display a message.
                //Application.Current.Shutdown();
            }
        }
    }
}
