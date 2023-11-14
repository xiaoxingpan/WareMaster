using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        private WareMasterEntities dbContext;
        public ResetPassword()
        {
            InitializeComponent();
            dbContext = Globals.DbContext;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TblOldPwd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtOldPwd.Focus();
        }

        private void TxtOldPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtOldPwd.Password) && TxtOldPwd.Password.Length > 0)
            {
                TblOldPwd.Visibility = Visibility.Hidden;
                TblAllInputErr.Visibility = Visibility.Hidden;
                TblOldPwdErr.Visibility = Visibility.Hidden;
            }
            else
            {
                TblOldPwd.Visibility = Visibility.Visible;
            }
        }

        private void TblNewPwd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtNewPwd.Focus();
        }

        private void TxtNewPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtNewPwd.Password) && TxtNewPwd.Password.Length > 0)
            {
                TblNewPwd.Visibility = Visibility.Collapsed;
                TblAllInputErr.Visibility = Visibility.Hidden;
                TblNewPwdErr.Visibility = Visibility.Hidden;
            }
            else
            {
                TblNewPwd.Visibility = Visibility.Visible;
            }
        }

        private void TblRepeatPwd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtRepeatPwd.Focus();
        }

        private void TxtRepeatPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtRepeatPwd.Password) && TxtRepeatPwd.Password.Length > 0)
            {
                TblRepeatPwd.Visibility = Visibility.Hidden;
                TblAllInputErr.Visibility = Visibility.Hidden;
                TblResetPwdErr.Visibility = Visibility.Hidden;
            }
            else
            {
                TblRepeatPwd.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string oldPwd = TxtOldPwd.Password.Trim();
                string newPwd = TxtNewPwd.Password.Trim();
                string repeatPwd = TxtRepeatPwd.Password.Trim();
                if (string.IsNullOrEmpty(TxtOldPwd.Password) || string.IsNullOrEmpty(TxtNewPwd.Password) || string.IsNullOrEmpty(TxtRepeatPwd.Password))
                {
                    TblAllInputErr.Visibility = Visibility.Visible;
                }
                else
                {
                    var user = Globals.wareMasterEntities.Users.FirstOrDefault(u => u.id == Globals.Id);
                    if (user != null)
                    {
                        string passwordSaved = user.Password;
                        string hashPassword = SetHashedPassword(oldPwd);
                        if (hashPassword!=passwordSaved)
                        {
                            TblOldPwdErr.Text = "Password is not correct!";
                            TblOldPwdErr.Visibility = Visibility.Visible;
                            return;
                        }
                        else
                        {
                            if (newPwd != repeatPwd)
                            {
                                TblResetPwdErr.Visibility = Visibility.Visible;
                                return;
                            }
                            else
                            {
                                user.Password = SetHashedPassword(newPwd);
                                dbContext.SaveChanges();
                                MessageBox.Show(this,"Your password succesfully reseted.","Message",MessageBoxButton.OK,MessageBoxImage.Information);
                                this.Close();
                            }
                        }

                        
                    }
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

        
    }
}
