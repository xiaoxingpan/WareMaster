using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for InventoryQuery.xaml
    /// </summary>
    public partial class InventoryQuery : Window
    {
        public InventoryQuery()
        {
            InitializeComponent();
            DateBegin.SelectedDate = DateTime.Now;
            DateEnd.SelectedDate = DateTime.Now;
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            disableDeleteMenu();
            DataGridResult.Tag = "";
            switch (QueryFor.Text)
            {
                case "Inventory":
                    
                    if (GroupBy.Text != "Category")
                    {
                        //Inventory By Item
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllInventoriesByItem((DateTime)DateBegin.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = new List<InventoryData> { Inventory.GetInventoryByItem((Item)txtInputName.Tag, (DateTime)DateBegin.SelectedDate) };
                        }
                    }
                    else
                    {
                        //Inventory By Category
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllInventoriesByCategory((DateTime)DateBegin.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = new List<InventoryData> { Inventory.GetInventoryByCategory((Category)txtInputName.Tag, (DateTime)DateBegin.SelectedDate) };
                        }
                    };
                    break;
                case "Summary":
                    if (GroupBy.Text != "Category")
                    {
                        //Summary By Item
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllSummaryByItem((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = Inventory.GetSummaryByItem((Item)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                    }
                    else
                    {
                            //Summary By Category
                            if (txtInputName.Tag == null)
                            {
                                DataGridResult.ItemsSource = Inventory.GetAllSummaryByCategory((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                            }
                            else
                            {
                                DataGridResult.ItemsSource = Inventory.GetSummaryByCategory((Category)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                            }
                        };
                    break;
                case "Details":
                    DataGridResult.Tag = "Details";
                    if (GroupBy.Text != "Category")
                    {
                        //Details By Item
                        if (txtInputName.Tag == null)
                        {
                            //query details of all items
                            DataGridResult.ItemsSource = Inventory.GetAllInventoryChangeDetailsByItem((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                        else
                        {
                            //query details of specified item
                            DataGridResult.ItemsSource = Inventory.GetInventoryChangeDetailsByItem((Item)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                    }
                    else
                    {
                        //Details By Category
                        if (txtInputName.Tag == null)
                        {
                            //no specified category, query details of all items
                            DataGridResult.ItemsSource = Inventory.GetAllInventoryChangeDetailsByItem((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                        else
                        {
                            //query details of specified category
                            DataGridResult.ItemsSource = Inventory.GetInventoryChangeDetailsByCategory((Category)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                    };
                    if (DataGridResult.Items.Count > 0)
                    {
                        enableDeleteMenu();
                    }
                    break;
                default: break;
            }
            

            foreach (DataGridTextColumn column in DataGridResult.Columns)
            {
                column.Binding.StringFormat = column.Header.ToString() == "Date" ? "yyyy-MM-dd" : column.Header.ToString() == "Total" ? "${0:N2}" : null;
            }

            Mouse.OverrideCursor = null;
        }

        private void QueryFor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblTo!=null && DateEnd!=null)
            {
                lblTo.Visibility = (QueryFor.SelectedIndex ==0) ? Visibility.Hidden: Visibility.Visible;
                DateEnd.Visibility = (QueryFor.SelectedIndex == 0) ? Visibility.Hidden : Visibility.Visible;
            }
            
        }

        private void txtInputName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchKeyword = txtInputName.Text;
            switch (GroupBy.Text)
            {
                case "Item":
                    List<Item> matchingItems = Globals.wareMasterEntities.Items
                        .Where(item => item.Itemname.Contains(searchKeyword))
                        .ToList();
                    ListBoxNames.Items.Clear();
                    foreach (Item item in matchingItems)
                    {
                        ListBoxItem listBoxName=new ListBoxItem();
                        listBoxName.Content = item.Itemname;
                        listBoxName.Tag = item;
                        ListBoxNames.Items.Add(listBoxName);
                    }
                    break;
                case "Category":
                    List<Category> machingCategories=Globals.wareMasterEntities.Categories
                        .Where(c=>c.Category_Name.Contains(searchKeyword))
                        .ToList ();
                    ListBoxNames.Items.Clear();
                    foreach (Category cat in machingCategories)
                    {
                        ListBoxItem listBoxName=new ListBoxItem();
                        listBoxName.Content=cat.Category_Name;
                        listBoxName.Tag = cat;
                        ListBoxNames.Items.Add(listBoxName);
                    }
                    break;
                default:
                    break;
            }
            if (string.IsNullOrWhiteSpace(searchKeyword)||ListBoxNames.Items.Count==0)
            {
                ListPopup.IsOpen = false;
                txtInputName.Tag = null;
            }
            else
            {
                ListPopup.IsOpen = true;
            }
        }

        private void ListBoxNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxNames.SelectedItem==null) return;
            
            ListBoxItem listBoxItem = (ListBoxItem)ListBoxNames.SelectedItem;
            switch (GroupBy.Text)
            {
                case "Item":
                    Item selectedItem = (Item)listBoxItem.Tag;
                    txtInputName.Tag = selectedItem;
                    txtInputName.Text = selectedItem.Itemname;

                    break;
                case "Category":
                    Category selectedCategory = (Category)listBoxItem.Tag;
                    txtInputName.Tag = selectedCategory;
                    txtInputName.Text = selectedCategory.Category_Name;
                    break;
                default:
                    break;
            }
            ListBoxNames.Items.Clear();
            ListPopup.IsOpen = false;

        }

        private void GroupBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtInputName==null) return;
            txtInputName.Tag = null;
            txtInputName.Text = "";
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Initial Inventory Data");

                // get data from listview
                var data = DataGridResult.Items;
                if (data.Count <= 0)
                {
                    MessageBox.Show("No data to export",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    return;
                }
                PropertyInfo[] columnTypes = data[0].GetType().GetProperties();

                // write data to excel
                for (int col = 1; col <= columnTypes.Length; col++)
                {
                    worksheet.Cells[1, col].Value = columnTypes[col - 1].Name;
                    for (int row = 2; row < data.Count + 2; row++)
                    {
                        PropertyInfo property = columnTypes[col - 1];
                        var value = property.GetValue(data[row - 2], null);
                        if (property.Name == "Date")
                        {
                            //worksheet.Cells[row,col].Value = ((DateTime)value).ToString("yyyy-MM-dd");
                            worksheet.Cells[row, col].Value = ((DateTime)value);
                            worksheet.Cells[row, col].Style.Numberformat.Format = "YYYY-MM-DD";
                        }
                        else if (property.Name == "Total")
                        {
                            worksheet.Cells[row, col].Value = value;
                            // Set currency format for Total column
                            worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = value;
                        }
                    }
                }
                using (var cells = worksheet.Cells[1, 1, 1, columnTypes.Length ])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                using (var cells = worksheet.Cells[2, 1, data.Count + 1, columnTypes.Length])
                {
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

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

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument document = new FlowDocument();
                System.Windows.Documents.Table table = new System.Windows.Documents.Table();

                var data = DataGridResult.Items;
                if (data.Count > 0)
                {
                    PropertyInfo[] columnTypes = data[0].GetType().GetProperties();
                    List<string> columnHeaders = columnTypes.Select(p => p.Name).ToList();

                    TableRowGroup headerGroup = new TableRowGroup();
                    System.Windows.Documents.TableRow headerRow = new System.Windows.Documents.TableRow();
                    foreach (string columnHeader in columnHeaders)
                    {
                        table.Columns.Add(new TableColumn());
                        double contentWidth = printDialog.PrintableAreaWidth * 0.9;
                        table.Columns[table.Columns.Count - 1].Width = new GridLength(contentWidth / columnHeaders.Count);
                        headerRow.Cells.Add(new System.Windows.Documents.TableCell(new Paragraph(new Run(columnHeader))));
                    }
                    headerGroup.Rows.Add(headerRow);
                    table.RowGroups.Add(headerGroup);

                    foreach (var item in data)
                    {
                        TableRowGroup dataGroup = new TableRowGroup();
                        System.Windows.Documents.TableRow dataRow = new System.Windows.Documents.TableRow();
                        foreach (string columnHeader in columnHeaders)
                        {
                            PropertyInfo property = item.GetType().GetProperty(columnHeader);
                            object value = property?.GetValue(item, null);
                            string formatedValue = "";
                            if (value != null)
                            {
                                try
                                {
                                    switch (columnHeader)
                                    {
                                        case "Total":
                                            formatedValue = ((decimal)value).ToString("N2");
                                            break;
                                        case "Date":
                                            formatedValue = ((DateTime)value).ToString("yyyy-MM-dd");
                                            break;
                                        default:
                                            formatedValue = value.ToString();
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    formatedValue = "";
                                }

                            }
                            dataRow.Cells.Add(new System.Windows.Documents.TableCell(new Paragraph(new Run(formatedValue))));
                        }
                        dataGroup.Rows.Add(dataRow);
                        table.RowGroups.Add(dataGroup);
                    }
                    document.Blocks.Add(table);

                    document.PageWidth = printDialog.PrintableAreaWidth;
                    document.ColumnWidth = document.PageWidth;
                    IDocumentPaginatorSource paginator = document;
                    paginator.DocumentPaginator.PageSize = new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);


                    printDialog.PrintDocument(paginator.DocumentPaginator, "Data Printing");
                    MessageBox.Show("Print successfully",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No Data to print",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                }
            }
        }
        private void enableDeleteMenu()
        {
            DeleteMenu.Visibility = Visibility.Visible;
        }
        private void disableDeleteMenu()
        {
            DeleteMenu.Visibility = Visibility.Collapsed;
        }
 
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(DataGridResult.SelectedIndex.ToString());
            InventoryData inventoryData=DataGridResult.SelectedItem as InventoryData;
            if (inventoryData == null || 
                MessageBoxResult.Cancel == MessageBox.Show("Delete inventory change data " + inventoryData.ToString(), "Confirm", MessageBoxButton.OKCancel,MessageBoxImage.Warning)) 
            {
                return;
            }
            
            Transaction transaction=Globals.wareMasterEntities.Transactions.FirstOrDefault(t=>t.id==inventoryData.id);
            if (transaction != null)
            {
                try
                {
                    DateTime lastSettleDate=Inventory.GetLastSettleDate();
                    if (lastSettleDate>=transaction.Transaction_Date)
                    {
                        MessageBox.Show("Cannot remove transaction record after settle date.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                        return;
                    }
                    //DataGridResult.Items.Remove(inventoryData);
                   
                    Globals.wareMasterEntities.Transactions.Remove(transaction);
                    Globals.wareMasterEntities.SaveChanges(); 
                    List<InventoryData> list = (List<InventoryData>)DataGridResult.ItemsSource;
                    
                    DataGridResult.ItemsSource = null;
                    list.Remove(inventoryData);
                    DataGridResult.ItemsSource = list;
                    MessageBox.Show("Record removed.",
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
}
