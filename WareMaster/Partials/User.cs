using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace WareMaster
{
    public partial class User
    {
        public RoleEnum RoleEnum { get; set; }

        public override string ToString()
        {
            return $"User ID: {id}, Username: {Username}, Role: {RoleEnum}, Email: {Email}";
        }

        public void SetHashedPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha256.ComputeHash(bytes);
                Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static bool IsUserNameValid(string username, int index, int userid, out string error)
        {
            List<string> allNames = Globals.wareMasterEntities.Users.Select(user => user.Username.ToLower()).ToList();
            List<string> otherNames = Globals.wareMasterEntities.Users
            .Where(user => user.id != userid)
            .Select(user => user.Username.ToLower())
            .ToList();
            if (username.Length < 5 || username.Length > 45 || !Regex.IsMatch(username, "^[a-zA-Z]+$"))
            {
                error = "Username must be 5-45 characters long, only letters";
                return false;
            }
            else if (index == 0 && allNames.Contains(username.ToLower()) || index == 1 && otherNames.Contains(username.ToLower()))
            {
                error = "Username must be unique";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsPasswordValid(string password, out string error)
        {
            if (password.Length < 1 || password.Length > 64 || !Regex.IsMatch(password, "^[a-zA-Z0-9]+$"))
            {
                error = "Password must be 1-64 characters long, contain only letters and/or numbers";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsEmailValid(string email, out string error)
        {
            if (email.Length < 1 || email.Length > 200)
            {
                error = "Email must be valid";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsRoleValid(string role, out string error)
        {
            if (role == null)
            {
                error = "You must choose a role";
                return false;
            }
            error = null;
            return true;
        }
    }

}
