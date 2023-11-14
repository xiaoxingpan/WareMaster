using Microsoft.Win32;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for ItemsManagementDialog.xaml
    /// </summary>
    public partial class ItemsManagementDialog : Window
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPage = 0;
        private List<ViewItem> filterItems = new List<ViewItem>();
        private List<ViewItem> allItems = new List<ViewItem>();
        private string loginUser = Globals.Username;

        public ItemsManagementDialog()
        {
            InitializeComponent();
            InitializeLvItems();
            AddPagingButton();
            //TbUserName.Text = loginUser;
        }
        private void AddPagingButton()
        {
            if (StackPaging.Children.Count > totalPage + 2)
            {
                StackPaging.Children.RemoveRange(2, totalPage);
            }

            totalPage = (int)Math.Ceiling((double)filterItems.Count / pageSize);
            for (int i = 0; i < totalPage; i++)
            {
                Button newPageButton = new Button()
                {
                    Name = "newPageButton" + i + 1,
                    Content = i + 1,
                    Width = 15,
                    Height = 15,
                    FontSize = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                newPageButton.Click += NewPageButton_Click;
                StackPaging.Children.Insert(i + 2, newPageButton);
            }
        }

        private void NewPageButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            SetColor();
            clickedButton.Background = new SolidColorBrush(Colors.LightSkyBlue);
            String page = clickedButton.Content.ToString();
            if (int.TryParse(page, out currentPage))
            {
                DisplayPage(currentPage);
            }
            else
            {
                MessageBox.Show(this, "Somthing went wrong, will display first page of items.", "error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                DisplayPage(1);
            }
        }

        private void SetColor()
        {
            try
            {
                for (int i = 2; i < StackPaging.Children.Count - 2; i++)
                {
                    ((Button)StackPaging.Children[i]).Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                try
                {
                    currentPage--;
                    Button clickedButton = (Button)StackPaging.Children[currentPage + 1];
                    SetColor();
                    clickedButton.Background = new SolidColorBrush(Colors.LightSkyBlue);
                    DisplayPage(currentPage);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage)
            {
                try
                {
                    currentPage++;
                    //Button clickedButton = (Button)StackPaging.FindName("newPageButton3");
                    Button clickedButton = (Button)StackPaging.Children[currentPage + 1];
                    SetColor();
                    clickedButton.Background = new SolidColorBrush(Colors.LightSkyBlue);
                    DisplayPage(currentPage);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DisplayPage(int page)
        {
            var itemsToDisplay = filterItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            LvItems.ItemsSource = itemsToDisplay;
        }

        private void InitializeLvItems()
        {
            try
            {
                var query = from item in Globals.wareMasterEntities.Items
                            join category in Globals.wareMasterEntities.Categories
                            on item.Category_Id equals category.id
                            select new ViewItem
                            {
                                ItemId = item.id,
                                ItemName = item.Itemname,
                                Description = item.Description != null ? item.Description : string.Empty,
                                CategoryName = category.Category_Name,
                                Unit = item.Unit != null ? item.Unit : string.Empty,
                                Location = item.Location != null ? item.Location : string.Empty
                            };
                allItems = query.ToList();
                filterItems = allItems;
                DisplayPage(currentPage);
                TxblItemCount.Text = "Total " + query.Count().ToString() + " Items";
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
        
        private class ViewItem
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string CategoryName { get; set; }
            public string Unit { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public override string ToString()
            {
                return $"ItemId: {ItemId}, ItemName: {ItemName}, CategoryName: {CategoryName}, Unit: {Unit}, Location: {Location}, Description: {Description}";
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAddItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEditItemsDialog dialog = new AddEditItemsDialog();
                dialog.Owner = this;
                //dialog.ShowDialog();
                if (dialog.ShowDialog() == true)
                {
                    InitializeLvItems();
                    //LblMessage.Text = "Item added";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Items Data");

                // get data from listview
                var data = LvItems.Items;
                if (data.Count <= 0)
                {
                    MessageBox.Show("No data to export");
                    return;
                }
                PropertyInfo[] columnTypes = data[0].GetType().GetProperties();

                // write data to excel
                for (int col = 1; col < columnTypes.Length; col++)
                {
                    worksheet.Cells[1, col].Value = columnTypes[col - 1].Name;
                    for (int row = 2; row < data.Count + 2; row++)
                    {
                        PropertyInfo property = columnTypes[col - 1];
                        var value = property.GetValue(data[row - 2], null);
                        if (property.PropertyType == typeof(DateTime))
                        {
                            worksheet.Cells[row, col].Value = ((DateTime)value).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = value;
                        }
                    }
                }
                using (var cells = worksheet.Cells[1, 1, 1, columnTypes.Length - 1])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // set column style
                using (var cells = worksheet.Cells[2, 1, data.Count + 1, columnTypes.Length - 1])
                {
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // set width and height
                worksheet.Cells.AutoFitColumns();

                // save Excel file
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx|All Files|*.*",
                    DefaultExt = "xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var newFile = new FileInfo(saveFileDialog.FileName);
                    package.SaveAs(newFile);
                    MessageBox.Show("Data exported successfully!", "Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ViewItem selectedItem = LvItems.SelectedItem as ViewItem;
            if (selectedItem == null) return;
            Item currItem = new Item();
            currItem.id = selectedItem.ItemId;
            currItem.Itemname = selectedItem.ItemName;
            currItem.Description = selectedItem.Description;
            currItem.Unit = selectedItem.Unit;
            currItem.Location = selectedItem.Location;
            currItem.Category_Id = Globals.wareMasterEntities.Items.Where(item => item.id == selectedItem.ItemId).Select(item => item.Category_Id).SingleOrDefault();

            AddEditItemsDialog dialog = new AddEditItemsDialog(currItem);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                InitializeLvItems();
                //LblMessage.Text = "Item updated";
            }
            //LvItems_SelectionChanged(LvItems, new SelectionChangedEventArgs(Selector.SelectedEvent, new List<object>(), new List<object>()));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewItem selectedItem = LvItems.SelectedItem as ViewItem;
            if (selectedItem == null) return;


            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Item itemToDelete = Globals.wareMasterEntities.Items.SingleOrDefault(item => item.id == selectedItem.ItemId);
                if (itemToDelete != null)
                {
                    Globals.wareMasterEntities.Items.Remove(itemToDelete);
                    Globals.wareMasterEntities.SaveChanges();
                    //LblMessage.Text = "Item deleted";
                    InitializeLvItems();
                }
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtFilter.Text == "")
            {
                filterItems = allItems;
                currentPage = 1;
                DisplayPage(currentPage);
            }
            else
            {
                filterItems = new List<ViewItem>(from item in allItems
                                                      where item.ItemName.ToLower().Contains(txtFilter.Text.Trim().ToLower())
                                                 select item);
                currentPage = 1;
                AddPagingButton();
                DisplayPage(currentPage);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;
                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    IsMaximized = true;
                }
            }

        }

        private void BtnToHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

     
    }

}
