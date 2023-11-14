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
    /// Interaction logic for InventoryInitEdit.xaml
    /// </summary>
    public partial class InventoryInitEdit : Window
    {
        private dynamic initRecord;

        public InventoryInitEdit(dynamic initData)
        {
            InitializeComponent();
            initRecord = initData;

            ItemIdTextBlock.Text = initRecord.ItemId.ToString();
            ItemNameTextBlock.Text = initRecord.ItemName;
            CategoryNameTextBlock.Text = initRecord.CategoryName;
            UnitTextBlock.Text = initRecord.Unit;
            LocationTextBlock.Text = initRecord.Location;
            DescriptionTextBlock.Text = initRecord.Description;

            QuantityTextBox.Text = initRecord.Quantity.ToString();
            TotalTextBox.Text = initRecord.Total.ToString("0.00");
            SettleDateDatePicker.SelectedDate = initRecord.SettleDate;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //validate
            bool validated=true;
            if (string.IsNullOrWhiteSpace(QuantityTextBox.Text) || !IsPositiveInteger(QuantityTextBox.Text))
            {
                validated = false;
                if (QuantityErrorTextBlock.Visibility!=Visibility.Visible) { 
                    this.Height += 30; 
                    QuantityErrorTextBlock.Visibility = Visibility.Visible;
                    QuantityErrorTextBlock.Text = "Please enter a valid positive integer for Quantity.";
                }
                
                
            }
            else
            {
                if (QuantityErrorTextBlock.Visibility == Visibility.Visible)
                { 
                    this.Height -= 30;
                    QuantityErrorTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            if (string.IsNullOrWhiteSpace(TotalTextBox.Text) || !IsDecimalWithTwoDecimalsAndPositive(TotalTextBox.Text))
            {
                validated = false;
                if (TotalErrorTextBlock.Visibility != Visibility.Visible)
                {
                    TotalErrorTextBlock.Visibility = Visibility.Visible;
                    TotalErrorTextBlock.Text = "Please enter a valid Total Amount with up to 2 decimal.";
                    this.Height += 30;
                }
            }
            else
            {
                if (TotalErrorTextBlock.Visibility == Visibility.Visible)
                {
                    TotalErrorTextBlock.Visibility = Visibility.Collapsed;
                    this.Height -= 30;
                }
                    
            }
            if (!validated) { return; }

            //confirm
            if (MessageBoxResult.No == MessageBox.Show("Are you sure you want to save this settlement data?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                return;
            };
            int idToSave = initRecord.SettlementId;
            if (idToSave == -1)
            {
                //insert new settlement
                try
                {
                    InsertNewSettlementData();
                    MessageBox.Show("New settlement data inserted successfully.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error inserting new settlement data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //update settlement
                try
                {
                    UpdateSettlementData(idToSave);
                    MessageBox.Show("Settlement data updated successfully.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating settlement data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void InsertNewSettlementData()
        {
            Settlement newSettlement = new Settlement
            {
                Item_Id= initRecord.ItemId,
                Quantity = Convert.ToInt32(QuantityTextBox.Text),
                Total = Convert.ToDecimal(TotalTextBox.Text),
                Settle_Date = SettleDateDatePicker.SelectedDate ?? DateTime.Now 
            };

            Globals.wareMasterEntities.Settlements.Add(newSettlement);

            Mouse.OverrideCursor = Cursors.Wait;
            Globals.wareMasterEntities.SaveChanges();
            Mouse.OverrideCursor = null;
        }

        private void UpdateSettlementData(int id)
        {
            Settlement settlementToUpdate = Globals.wareMasterEntities.Settlements.FirstOrDefault(s => s.id == id);

            if (settlementToUpdate != null)
            {
                settlementToUpdate.Quantity = Convert.ToInt32(QuantityTextBox.Text);
                settlementToUpdate.Total = Convert.ToDecimal(TotalTextBox.Text);
                settlementToUpdate.Settle_Date = SettleDateDatePicker.SelectedDate ?? DateTime.Now;

                Mouse.OverrideCursor = Cursors.Wait;
                Globals.wareMasterEntities.SaveChanges();
                Mouse.OverrideCursor = null;

            }
        }
            private bool IsPositiveInteger(string input)
        {
            int number;
            return int.TryParse(input, out number) && number > 0;
        }

        private bool IsDecimalWithTwoDecimalsAndPositive(string input)
        {
            decimal number;
            if (decimal.TryParse(input, out number))
            {
                return number > 0 && decimal.Round(number, 2) == number;
            }
            return false;
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int idToDelete = initRecord.SettlementId;
            if (idToDelete == -1) {
                MessageBox.Show("No settlement data could be deleted!",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return; 
            }
            //confirm
            if(MessageBoxResult.No == MessageBox.Show("Are you sure you want to delete this settlement data?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question)){
                return;
            };
            //delete
            try
            {
                var settlementToDelete = Globals.wareMasterEntities.Settlements.Where(s => s.id == idToDelete).FirstOrDefault();
                if (settlementToDelete != null)
                {
                    Globals.wareMasterEntities.Settlements.Remove(settlementToDelete);
                    Mouse.OverrideCursor = Cursors.Wait;
                    Globals.wareMasterEntities.SaveChanges();
                    Mouse.OverrideCursor = null;
                    MessageBox.Show("Settlement data deleted successfully.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    this.Close(); 
                }
                else
                {
                    MessageBox.Show("Settlement data not found. Deletion failed.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deleting settlement data: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }

}
