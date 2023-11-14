using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace WareMaster
{
    public class UserInputValidator : AbstractValidator<User>
    {
        public UserInputValidator(int index, int userid)
        {
            RuleFor(User => User.Username).NotNull().NotEmpty().Length(5, 45).Matches("^[a-zA-Z]+$").Must((user, Username) => IsUsernameUnique(Username, index, userid))
            .WithMessage("Username must be unique");  // only contains letters
            RuleFor(User => User.Role).NotNull();
            RuleFor(User => User.Password).NotNull().NotEmpty().Length(1, 64).Matches("[a-zA-Z0-9]+$");  // only contains letters and numbers
            RuleFor(User => User.Email).NotNull().NotEmpty().Length(1, 200).EmailAddress();
        }

        private bool IsUsernameUnique(string username, int index, int userid)
        {
            try
            {
                List<string> namesToCheck;
                if (index == 0)
                {
                    namesToCheck = Globals.wareMasterEntities.Users.Select(user => user.Username.ToLower()).ToList();
                }
                else if (index == 1)
                {
                    namesToCheck = Globals.wareMasterEntities.Users
            .Where(user => user.id != userid)
            .Select(user => user.Username.ToLower())
            .ToList();
                }
                else
                {
                    return false;
                }
                return !namesToCheck.Contains(username.ToLower());

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
