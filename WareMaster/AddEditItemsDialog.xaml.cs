using FluentValidation;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for AddEditItemsDialog.xaml
    /// </summary>

    public partial class AddEditItemsDialog : Window
    {
        Item currItem = new Item();
        string errorMessage;
        int index = 0;  //add -> 0, edit -> 1
        List<Category> categories = Globals.wareMasterEntities.Categories.ToList();

        public AddEditItemsDialog(Item currItem = null)
        {
            this.currItem = currItem;
            InitializeComponent();
            InitializeCategory();
            if (currItem != null) // update, load select values
            {
                index = 1;
                ItemId.Text = currItem.id.ToString();
                ItemNameInput.Text = currItem.Itemname;
                DescriptionInput.Text = currItem.Description;
                CategoryComboBox.SelectedItem = categories.FirstOrDefault(category => category.id == currItem.Category_Id);
                foreach (ComboBoxItem item in UnitComboBox.Items)
                {
                    if (item.Content.ToString() == currItem.Unit)
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
                LocationInput.Text = currItem.Location.Substring(1);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void InitializeCategory()
        {
            try
            {
                CategoryComboBox.ItemsSource = categories;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string selectedCategoryName = (CategoryComboBox.SelectedItem as Category)?.Category_Name;
            Category selectedCategory = categories.FirstOrDefault(category => category.Category_Name == selectedCategoryName);
            try
            {
                if (string.IsNullOrWhiteSpace(ItemNameInput.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionInput.Text) ||
                    string.IsNullOrWhiteSpace(LocationInput.Text) ||
                    CategoryComboBox.SelectedItem == null ||
                    UnitComboBox.SelectedItem == null)
                {                  
                    throw new ArgumentException("Input incompleted");
                }

                if (currItem != null)
                { // update
                    Console.WriteLine("ENTER EDIT");
                    var itemToUpdate = Globals.wareMasterEntities.Items.FirstOrDefault(item => item.id == currItem.id);
                    itemToUpdate.Itemname = ItemNameInput.Text;
                    itemToUpdate.Description = DescriptionInput.Text;
                    itemToUpdate.Category_Id = selectedCategory.id;
                    itemToUpdate.Location = "A" + LocationInput.Text;
                    itemToUpdate.Unit =(UnitComboBox.SelectedItem as ComboBoxItem)?.Tag as string;
                    Console.WriteLine(currItem);
                    var validator = new ItemInputValidator(index, currItem.id);
                    var result = validator.Validate(itemToUpdate);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }

                }
                else // add
                {
                    Console.WriteLine("enter add");
                    Item newItem = new Item
                    {
                        Itemname = ItemNameInput.Text,
                        Description = DescriptionInput.Text,

                        Category_Id = selectedCategory.id,
                        Location = "A" + LocationInput.Text,
                        Unit = (UnitComboBox.SelectedItem as ComboBoxItem)?.Tag as string

                    };
                    var validator = new ItemInputValidator(index, 0);
                    var result = validator.Validate(newItem);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }
                    Console.WriteLine(newItem);
                    Globals.wareMasterEntities.Items.Add(newItem);
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

        private void ItemNameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            int itemIdToCheck = (currItem != null) ? currItem.id : 0;

            if (!Item.IsItemNameValid(ItemNameInput.Text, index, itemIdToCheck, out errorMessage))
            {
                LblErrItemName.Visibility = Visibility.Visible;
                LblErrItemName.Text = errorMessage;
            }
            else
            {
                LblErrItemName.Visibility = Visibility.Hidden;
            }
        }

        private void DescriptionInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsDescriptionValid(DescriptionInput.Text, out errorMessage))
            {
                LblErrDescription.Visibility = Visibility.Visible;
                LblErrDescription.Text = errorMessage;
            }
            else
            {
                LblErrDescription.Visibility = Visibility.Hidden;
            }
        }

        private void CategoryComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsCategoryValid(CategoryComboBox.SelectedItem.ToString(), out errorMessage))
            {
                LblErrCategory.Visibility = Visibility.Visible;
                LblErrCategory.Text = errorMessage;
            }
            else
            {
                LblErrCategory.Visibility = Visibility.Hidden;
            }
        }

        private void LocationInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(LocationInput.Text, out int aisle))
            {
                if (!Item.IsLocationValid(aisle, out errorMessage))
                {
                    LblErrLocation.Visibility = Visibility.Visible;
                    LblErrLocation.Text = errorMessage;
                }
                else
                {
                    LblErrLocation.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                LblErrLocation.Visibility = Visibility.Visible;
                LblErrLocation.Text = "Invalid aisle format. Please enter a valid integer.";
            }
        }

        private void UnitComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsUnitValid(UnitComboBox.SelectedItem.ToString(), out errorMessage))
            {
                LblErrUnit.Visibility = Visibility.Visible;
                LblErrUnit.Text = errorMessage;
            }
            else
            {
                LblErrUnit.Visibility = Visibility.Hidden;
            }
        }
    }

}
