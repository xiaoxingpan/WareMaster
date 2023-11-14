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
    /// Interaction logic for InventorySettle.xaml
    /// </summary>
    public partial class InventorySettle : Window
    {
        public InventorySettle()
        {
            InitializeComponent();
            ShowSettletDates(5);
            RemoveOld.Visibility =(Globals.Role != RoleEnum.ADMIN)?Visibility.Collapsed:Visibility.Visible;
        }
        private void ShowSettletDates(int numOfRecords)
        {
            List<DateTime> recentSettlementDates = Globals.wareMasterEntities
                .Settlements
                .Select(s => s.Settle_Date)
                .Distinct()
                .OrderByDescending(date=>date)
                .Take(numOfRecords)
                .ToList();
            LVSettle.ItemsSource = recentSettlementDates;

        }
        private void GetSettleHistory_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords>0)
            {
                ShowSettletDates(numOfRecords);
            }
            else
            {
                MessageBox.Show("Please enter a valid number of records.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            Mouse.OverrideCursor = null;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (LVSettle.SelectedItem == null)
            {
                MessageBox.Show("Please select a settlement date to delete.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Mouse.OverrideCursor = null;
                return;
            }
            if(MessageBoxResult.No== MessageBox.Show("Are you sure you want to delete the selected settlement date?", "Confirmation", MessageBoxButton.YesNo,MessageBoxImage.Question))
            {
                Mouse.OverrideCursor = null;
                return;
            }
            DateTime selectedDate = (DateTime)LVSettle.SelectedItem;
            try
            {
                DateTime minDate = Globals.wareMasterEntities.Settlements
                    .Select(s => s.Settle_Date)
                    .DefaultIfEmpty(DateTime.MaxValue) 
                    .Min();

                if (selectedDate <= minDate)
                {
                    MessageBox.Show("Cannot delete the earlist settlement data.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    Mouse.OverrideCursor = null;
                    return;
                }

                var settlementsToDelete = Globals.wareMasterEntities.Settlements
                    .Where(s => s.Settle_Date == selectedDate)
                    .ToList();

                Globals.wareMasterEntities.Settlements.RemoveRange(settlementsToDelete);
                Globals.wareMasterEntities.SaveChanges();
                if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords > 0)
                {
                    ShowSettletDates(numOfRecords);
                }
                else
                {
                    ShowSettletDates(5);
                }
                MessageBox.Show("Settlement deleted successfully.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error deleting settlement: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }

        private void Settle_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            DateTime lastSettleDate = Globals.wareMasterEntities.Settlements
                .Select(s => s.Settle_Date)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();
            DateTime settleDate = (DateTime)dpSettleDate.SelectedDate;
            if (settleDate <= lastSettleDate)
            {
                MessageBox.Show("Cannot settle on or before the last settlement date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Mouse.OverrideCursor = null; 
                return;
            }
            if (MessageBoxResult.No==MessageBox.Show("Do you want to settle inventories?", "Confirmation", MessageBoxButton.YesNo,MessageBoxImage.Question))
            {
                Mouse.OverrideCursor = null; 
                return;
            }

            //foreach (var item in Globals.wareMasterEntities.Items)
            //{
            //// find last settlement of the item
            //Settlement lastSettlement = Globals.wareMasterEntities.Settlements
            //    .Where(s => s.Item_Id == item.id)
            //    .OrderByDescending(s => s.Settle_Date)
            //    .FirstOrDefault();

            //if (lastSettlement != null)
            //{
            //    // get sum of transactions
            //    List<Transaction> transactionsAfterLastSettle = Globals.wareMasterEntities.Transactions
            //        .Where(t => t.Item_Id == item.id && t.Transaction_Date > lastSettlement.Settle_Date)
            //        .ToList();

            //    int totalQuantity = transactionsAfterLastSettle.Sum(transaction => transaction.Quantity);
            //    decimal totalTotal = transactionsAfterLastSettle.Sum(transaction => transaction.Total);


            //    // new settlement
            //    Settlement newSettlement = new Settlement
            //    {
            //        Item_Id = item.id,
            //        Settle_Date = dpSettleDate.SelectedDate.Value,
            //        Quantity = lastSettlement.Quantity + totalQuantity,
            //        Total = lastSettlement.Total + totalTotal
            //    };
            //    Globals.wareMasterEntities.Settlements.Add(newSettlement);
            //}
            //else
            //{
            //    //get sum of transactions
            //    List<Transaction> transactionsAfterLastSettle = Globals.wareMasterEntities.Transactions
            //        .Where(t => t.Item_Id == item.id )
            //        .ToList();

            //    int totalQuantity = transactionsAfterLastSettle.Sum(transaction => transaction.Quantity);
            //    decimal totalTotal = transactionsAfterLastSettle.Sum(transaction => transaction.Total);
            //    // new settlement
            //    Settlement newSettlement = new Settlement
            //    {
            //        Item_Id = item.id,
            //        Settle_Date = dpSettleDate.SelectedDate.Value,
            //        Quantity = totalQuantity, 
            //        Total = totalTotal 
            //    };
            //    Globals.wareMasterEntities.Settlements.Add(newSettlement);
            //}

            //}
            //add settlements
            List<InventoryData> inventorys = Inventory.GetAllInventoriesByItem(settleDate);
            foreach (InventoryData inventory in inventorys)
            {
                Settlement newSettlement = new Settlement
                {
                    Item_Id = inventory.id,
                    Settle_Date = settleDate,
                    Quantity = inventory.Quantity,
                    Total = inventory.Total,
                };
                Globals.wareMasterEntities.Settlements.Add(newSettlement);
            }
            // save changes
            try
            {
                Globals.wareMasterEntities.SaveChanges();

                if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords > 0)
                {
                    ShowSettletDates(numOfRecords);
                }
                else
                {
                    ShowSettletDates(5);
                }
                MessageBox.Show("Settlement completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error settling inventories: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void RemoveOld_Click(object sender, RoutedEventArgs e)
        {
            
            if (LVSettle.SelectedItem == null)
            {
                MessageBox.Show("Please select a settle date",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            DateTime settleDate = (DateTime)LVSettle.SelectedItem;
            if (MessageBoxResult.No == MessageBox.Show($"Are you sure to remove all settlement and transaction data before {settleDate.Date:yyyy-MM-dd}?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning))
            {
                return;
            }
            try
            {
                Globals.wareMasterEntities.Transactions.RemoveRange(
                    Globals.wareMasterEntities.Transactions.Where(t => t.Transaction_Date <= settleDate));
                Globals.wareMasterEntities.Settlements.RemoveRange(
                    Globals.wareMasterEntities.Settlements.Where(s => s.Settle_Date < settleDate));
                Globals.wareMasterEntities.SaveChanges();
                if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords > 0)
                {
                    ShowSettletDates(numOfRecords);
                }
                else
                {
                    ShowSettletDates(5);
                }
                MessageBox.Show("Data removed successfully.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
