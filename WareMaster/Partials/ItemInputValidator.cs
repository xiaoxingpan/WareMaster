using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;


namespace WareMaster
{
    public class ItemInputValidator : AbstractValidator<Item>
    {
        public ItemInputValidator(int index, int itemid)
        {
            RuleFor(Item => Item.Itemname).NotNull().NotEmpty().Length(1, 200).Matches("^[a-zA-Z]+$").Must((item, Itemname) => IsItemnameUnique(Itemname, index, itemid))
            .WithMessage("Itemname must be unique");  // only contains letters
            RuleFor(Item => Item.Description).NotNull().NotEmpty().Length(1, 500).Matches("^[a-zA-Z\\s]+$");  // only contains letters and/or space
            RuleFor(Item => Item.Category_Id).NotNull().NotEmpty().Must((item, Category_Id) => IsCategoryIdExist(Category_Id)).WithMessage("Category does not exist.");
            RuleFor(Item => Item.Unit).NotNull().NotEmpty();
            RuleFor(Item => Item.Location).NotNull().NotEmpty().Matches("^A([1-9]|[1-4][0-9]|60)$");  // A1 
        }

        private bool IsItemnameUnique(string itemname, int index, int itemid)
        {
            try
            {
                List<string> namesToCheck;
                if (index == 0)
                {
                    namesToCheck = Globals.wareMasterEntities.Items.Select(item => item.Itemname.ToLower()).ToList();
                }
                else if (index == 1)
                {
                    namesToCheck = Globals.wareMasterEntities.Items
            .Where(item => item.id != itemid)
            .Select(item => item.Itemname.ToLower())
            .ToList();
                }
                else
                {
                    return false;
                }
                return !namesToCheck.Contains(itemname.ToLower());

            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsCategoryIdExist(int categoryid) 
        {
            try
            {
                List<int> allIds = Globals.wareMasterEntities.Categories.Select(category => category.id).ToList();
                return allIds.Contains(categoryid);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
