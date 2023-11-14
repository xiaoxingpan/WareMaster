using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for AddEditCategoryDialog.xaml
    /// </summary>
    public partial class AddEditCategoryDialog : Window
    {
        Category currCategory = new Category();
        string errorMessage;
        int index = 0; // add

        public AddEditCategoryDialog(Category currCategory = null)
        {
            InitializeComponent();
            this.currCategory = currCategory;

            if (currCategory != null) // update, load select values
            {
                index = 1;
                CategoryId.Text = currCategory.id.ToString();
                CategorynameInput.Text = currCategory.Category_Name;
            }
        }

        private void CategorynameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            int idToCheck = (currCategory != null) ? currCategory.id : 0;
            if (!User.IsUserNameValid(CategorynameInput.Text, index, idToCheck, out errorMessage))
            {
                TbxErrCategoryname.Visibility = Visibility.Visible;
                TbxErrCategoryname.Text = errorMessage;
            }
            else
            {
                TbxErrCategoryname.Visibility = Visibility.Hidden;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CategorynameInput.Text))
                {
                    throw new ArgumentException("Input incompleted");
                }

                if (currCategory != null)
                {
                    // update
                    var categoryToUpdate = Globals.wareMasterEntities.Categories.FirstOrDefault(category => category.id == currCategory.id);
                    categoryToUpdate.Category_Name = CategorynameInput.Text;
                    var validator = new CategoryInputValidator(index, currCategory.id);
                    var result = validator.Validate(categoryToUpdate);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }
                    Console.WriteLine(currCategory);
                }
                else
                {
                    // add
                    Category newCategory = new Category
                    {
                        Category_Name = CategorynameInput.Text
                    };
                    var validator = new CategoryInputValidator(index, 0);
                    var result = validator.Validate(newCategory);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }
                    Console.WriteLine(newCategory);
                    Globals.wareMasterEntities.Categories.Add(newCategory);
                }
                Globals.wareMasterEntities.SaveChanges();
                this.DialogResult = true; // dismiss the dialog
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, ex.Message, "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Database error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
