using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private WareMasterEntities dbContext;
        public bool IsAuthenticated{get; set;}
        
        public Login()
        {
            InitializeComponent();
            dbContext = Globals.DbContext;
            IsAuthenticated = false;
        }

        private void TblName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtName.Focus();
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtName.Text) && TxtName.Text.Length > 0)
            {
                TblName.Visibility = Visibility.Hidden;
                TblUsernameErr.Visibility = Visibility.Hidden;
            }
            else
            {
                TblName.Visibility = Visibility.Visible;
            }

        }

        private void TblPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtPassword.Focus();
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtPassword.Password) && TxtPassword.Password.Length > 0)
            {
                TblPassword.Visibility = Visibility.Hidden;
                TblPasswordeErr.Visibility = Visibility.Hidden;
            }
            else
            {
                TblPassword.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtName.Text) && !string.IsNullOrEmpty(TxtPassword.Password))
                {
                    string username = TxtName.Text.Trim();
                    string password = TxtPassword.Password.Trim();
                    string passwordSaved = "";
                    int id = 0;
                    RoleEnum role;
                    var user = dbContext.Users
                        .Where(u => u.Username == username)
                        .Select(u => new
                        {
                            u.id,
                            u.Password,
                            u.Role
                        })
                        .FirstOrDefault(); // Use FirstOrDefault to get a single result or null if not found
                    if (user==null)
                    {
                        TblUsernameErr.Text = "User Name does not exist!";
                        TblUsernameErr.Visibility = Visibility.Visible;
                        return;
                    }
                    if (user != null)
                    {
                        id = user.id;
                        passwordSaved = user.Password;
                        role = user.Role;
                        string hashPassword = SetHashedPassword(password);
                        if (hashPassword!=passwordSaved)
                        {
                            TblPasswordeErr.Text = "Password is not correct!";
                            TblPasswordeErr.Visibility = Visibility.Visible;
                            return;
                        }
                        Globals.Id = id;
                        Globals.Username = username;
                        Globals.Role = (RoleEnum)role;
                        IsAuthenticated = true;
                        WMLogger.WriteLog("User ID:"+ id+ " logged in");
                        Close();
                    }
                    
                }
            }
            catch(SystemException ex)
            {
                Console.WriteLine(ex.ToString());
                WMLogger.WriteLog(ex.ToString());
            }
            
        }

        public string SetHashedPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
