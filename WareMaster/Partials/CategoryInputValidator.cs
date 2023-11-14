using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public class CategoryInputValidator : AbstractValidator<Category>
    {
        public CategoryInputValidator(int index, int categoryid) 
        {
            RuleFor(Category => Category.Category_Name).NotNull().NotEmpty().Length(1, 200).Matches("^[a-zA-Z]+$").Must((category, Category_Name) => IsCategorynameUnique(Category_Name, index, categoryid))
                .WithMessage("Category name must be unique");  // only contains letters
        }
        private bool IsCategorynameUnique(string categoryname, int index, int categoryid)
        {
            try
            {
                List<string> namesToCheck;
                if (index == 0)
                {
                    namesToCheck = Globals.wareMasterEntities.Categories.Select(category => category.Category_Name.ToLower()).ToList();
                }
                else if (index == 1)
                {
                    namesToCheck = Globals.wareMasterEntities.Categories
           .Where(category => category.id != categoryid)
           .Select(category => category.Category_Name.ToLower())
           .ToList();
                }
                else
                {
                    return false;
                }
                return !namesToCheck.Contains(categoryname.ToLower());

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
