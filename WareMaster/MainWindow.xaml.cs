
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    //public class ExitKey : ICommand
    //{
    //    public event EventHandler CanExecuteChanged;

    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        Application.Current.Shutdown();
    //    }
    //}

    //public class ExitCommandContext
    //{
    //    public ICommand ExitCommand
    //    {
    //        get
    //        {
    //            return new ExitKey();
    //        } 
    //    }
    //}
    public partial class MainWindow : Window
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPage = 0;
        private RoleEnum role;
        //private List<ItemViewModel> allItems = new List<ItemViewModel>() ;
        //private List<ItemViewModel> filterItems = new List<ItemViewModel>();
        private List<InventoryData> allItems=new List<InventoryData>();
        private List<InventoryData> filterItems=new List<InventoryData>();
        private WareMasterEntities dbContext;
        public MainWindow()
        {
            InitializeComponent();
            SwitchLanguage("En");
            initMainWindow();
            Cmbxlanguage.SelectedIndex=0;
        }

        private void initMainWindow()
        {
            if (Globals.Role==RoleEnum.ADMIN)
            {
                BtnManagerUser.IsEnabled = true;
            }
            else
            {
                BtnManagerUser.IsEnabled=false;
            }
            //this.DataContext = new ExitCommandContext();
            //InitializeComponent();
            //Globals.wareMasterEntities = new WareMasterEntities();
            dbContext = Globals.DbContext;
            InitializeLvInit();
            AddPagingButton();
            TblWelcome.Text = TblWelcome.Text + " "+Globals.Username;
        }

        private void AddPagingButton()
        {
            if (StackPaging.Children.Count> totalPage+2)
            {
                StackPaging.Children.RemoveRange(2, totalPage);
            }
            
            totalPage = (int)Math.Ceiling((double)filterItems.Count / pageSize);
            for (int i = 0; i < totalPage; i++)
            {
                Button newPageButton = new Button()
                {
                    Name = "newPageButton"+i+1,
                    Content = i+1,
                    Width = 15,
                    Height = 15,
                    FontSize = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                newPageButton.Click += NewPageButton_Click;
                StackPaging.Children.Insert(i+2, newPageButton);
            }
        }
        private void NewPageButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton =(Button)sender;
            SetColor();
            clickedButton.Background = new SolidColorBrush(Colors.LightSkyBlue);
            String page = clickedButton.Content.ToString();
            if (int.TryParse(page, out currentPage))
            {
                DisplayPage(currentPage);
            }
            else
            {
                MessageBox.Show(this,"Somthing went wrong, will display first page of items.","error",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                DisplayPage(1);
            }
        }

        private void SetColor()
        {
            try
            {
                for (int i = 2; i<StackPaging.Children.Count -2; i++)
                {
                    ((Button)StackPaging.Children[i]).Background = new SolidColorBrush(Colors.Transparent);
                }
            }catch (SystemException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            
        }
        private void DisplayPage(int page)
        {
            var itemsToDisplay = filterItems.Skip((page-1)*pageSize).Take(pageSize).ToList();
            DgStorage.ItemsSource = itemsToDisplay;
        }

        private void InitializeLvInit()
        {
            try
            {
                //var query = from item in dbContext.Items
                //            join settlement in dbContext.Settlements on item.id equals settlement.Item_Id into gj
                //            from sub in gj.DefaultIfEmpty()
                //            select new ItemViewModel
                //            {
                //                ItemId = item.id,
                //                ItemName = item.Itemname,
                //                CategoryName = item.Category.Category_Name,
                //                Unit = item.Unit != null ? item.Unit : string.Empty,
                //                Location = item.Location != null ? item.Location : string.Empty,
                //                Description = item.Description != null ? item.Description : string.Empty,
                //                Quantity = sub != null ? sub.Quantity : 0,
                //                Total = sub != null ? sub.Total : 0,
                //                SettleDate = sub != null ? sub.Settle_Date : DateTime.Now,
                //                SettlementId = sub != null ? sub.id : -1
                //            };

                //allItems = query.ToList();
                allItems = Inventory.GetAllInventoriesByItem(DateTime.Now.Date);
                filterItems = allItems;
                DisplayPage(currentPage);
                //TxblItemCount.Text = "Total " + query.Count().ToString() + " Items";
                TxblItemCount.Text = (string)this.FindResource("totalItems")  + " "+ allItems.Count().ToString();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryInit inventoryInit = new InventoryInit();
                inventoryInit.Owner = this;
                inventoryInit.ShowDialog();
            }catch (Exception ex) { MessageBox.Show(ex.Message); };
            
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
            if (e.ClickCount==2)
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
                    //GridContent.Height = 920;
                    IsMaximized = true;
                }
            }

        }
        //private void MenuItemDataBackup_Click(object sender, RoutedEventArgs e)
        //{

        //}
        //private void MenuItemDataRecory_Click(object sender, RoutedEventArgs e)
        //{

        //}
        private void MenuItemInventoryInit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryInit inventoryInit = new InventoryInit();
                inventoryInit.Owner = this;
                //inventoryInit.Left = this.Left + (this.Width - inventoryInit.Width) / 2;
                //inventoryInit.Top = this.Top + (this.Height - inventoryInit.Height) / 2;
                inventoryInit.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemInventoryInbound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryInbound = new InventoryChange("Inbound");
                inventoryInbound.Owner = this;
                //inventoryInbound.Left = this.Left + (this.Width - inventoryInbound.Width) / 2;
                //inventoryInbound.Top = this.Top + (this.Height - inventoryInbound.Height) / 2;
                inventoryInbound.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemInventoryOutbound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryOutbound = new InventoryChange("Outbound");
                inventoryOutbound.Owner = this;
                //inventoryOutbound.Left = this.Left + (this.Width - inventoryOutbound.Width) / 2;
                //inventoryOutbound.Top = this.Top + (this.Height - inventoryOutbound.Height) / 2;
                inventoryOutbound.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemInventorySettle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventorySettle inventorySettle = new InventorySettle();
                inventorySettle.Owner = this;
                //inventorySettle.Left = this.Left + (this.Width - inventorySettle.Width) / 2;
                //inventorySettle.Top = this.Top + (this.Height - inventorySettle.Height) / 2;
                inventorySettle.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryQuery inventoryQuery = new InventoryQuery();
                inventoryQuery.Owner = this;
                //inventoryQuery.Left = this.Left + (this.Width - inventoryQuery.Width) / 2;
                //inventoryQuery.Top = this.Top + (this.Height - inventoryQuery.Height) / 2;
                inventoryQuery.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void MenuManageItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemsManagementDialog itemDialog = new ItemsManagementDialog();
                itemDialog.Owner = this;
                itemDialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void MenuItemNewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEditItemsDialog dialog = new AddEditItemsDialog();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void MenuItemUpdateItem_Click(object sender, RoutedEventArgs e)
        {
            //ItemViewModel selectedItem = DgStorage.SelectedItem as ItemViewModel;
            //if (selectedItem == null) return;
            //Item currItem = new Item();
            //currItem.id = selectedItem.ItemId;
            //currItem.Itemname = selectedItem.ItemName;
            //currItem.Description = selectedItem.Description;
            //currItem.Unit = selectedItem.Unit;
            //currItem.Location = selectedItem.Location;
            //currItem.Category_Id = Globals.wareMasterEntities.Items.Where(item => item.id == selectedItem.ItemId).Select(item => item.Category_Id).SingleOrDefault();
            InventoryData selectedItem=DgStorage.SelectedItem as InventoryData;
            if (selectedItem == null) return;

            Item currItem = Globals.wareMasterEntities.Items.FirstOrDefault(item => item.id == selectedItem.id);
            AddEditItemsDialog dialog = new AddEditItemsDialog(currItem);
            dialog.Owner = this;
                dialog.ShowDialog();
        }

        private void MenuItemAnalyse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Analyse dialog = new Analyse();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void MenuItemUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersManagementDialog dialog = new UsersManagementDialog();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CommandBindingNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void BtnManageItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemsManagementDialog itemDialog = new ItemsManagementDialog();
                itemDialog.Owner = this;
                itemDialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void BtnManageCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoriesManagementDialog itemDialog = new CategoriesManagementDialog();
                itemDialog.Owner = this;
                itemDialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnManageUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersManagementDialog itemDialog = new UsersManagementDialog();
                itemDialog.Owner = this;
                itemDialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                try
                {
                    currentPage--;
                    Button clickedButton = (Button)StackPaging.Children[currentPage+1];
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
                    Button clickedButton = (Button)StackPaging.Children[currentPage+1];
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



        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show("3 currentPage="+currentPage);
            if (txtFilter.Text=="")
            {
                filterItems = allItems;
                currentPage = 1;
                AddPagingButton();
                DisplayPage(currentPage);
            }
            else
            {
                //filterItems = new List<ItemViewModel>(from item in allItems
                //              where item.ItemName.ToLower().Contains(txtFilter.Text.ToLower().Trim())
                //              select item);
                filterItems=new List<InventoryData>(from item in allItems
                                                    where item.Name.ToLower().Contains(txtFilter.Text.ToLower().Trim())
                                                    select item);
                currentPage = 1;
                AddPagingButton();
                DisplayPage(currentPage);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //base.OnClosed(e);
            App.Current.Shutdown(0);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            DisableControls();
        }

        private void DisableControls()
        {
            BtnLogin.Visibility = Visibility.Visible;
            BtnLogout.Visibility = Visibility.Hidden;
            MainMenu.IsEnabled = false;
            BtnManageCategory.IsEnabled = false;
            BtnInbound.IsEnabled = false;
            BtnOutbound.IsEnabled = false;
            BtnManageItem.IsEnabled = false;
            BtnManagerUser.IsEnabled = false;
            txtFilter.IsEnabled = false;
            DgStorage.IsEnabled = false;
            TblWelcome.Text = (string)this.FindResource("Welcome");
            Globals.Username = "";
            filterItems.Clear();

            StackPaging.Children.RemoveRange(2,totalPage);

            DisplayPage(1);
            
        }

        private void EnableControls()
        {
            BtnLogin.Visibility = Visibility.Hidden;
            BtnLogout.Visibility = Visibility.Visible;
            MainMenu.IsEnabled = true;
            BtnManageCategory.IsEnabled = true;
            BtnInbound.IsEnabled = true;
            BtnOutbound.IsEnabled = true;
            BtnManageItem.IsEnabled = true;
            BtnManagerUser.IsEnabled = true;
            txtFilter.IsEnabled = true;
            DgStorage.IsEnabled = true;
            initMainWindow();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Login loginWindow = new Login();
                loginWindow.ShowDialog();
                loginWindow.Owner = this;
                if (loginWindow.IsAuthenticated)
                {
                    EnableControls();
                }
                else
                {
                    MessageBox.Show("Your are not logged in.");
                    //Application.Current.Shutdown();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnResetPwd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetPassword resetPwd = new ResetPassword();
                resetPwd.ShowDialog();
                resetPwd.Owner = this;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void Cmbxlanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lang = (String)((ComboBoxItem)Cmbxlanguage.SelectedItem).Tag;
            SwitchLanguage(lang);
            TxblItemCount.Text = (string)this.FindResource("totalItems")  + " "+ allItems.Count().ToString();
            TblWelcome.Text = (string)this.FindResource("Welcome") +" "+Globals.Username;
        }

        private void SwitchLanguage(string lang)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case "En":
                    dict.Source = new Uri("..\\StringResource.en.xaml", UriKind.Relative);
                    break;
                case "Fr":
                    dict.Source = new Uri("..\\StringResource.fr.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\StringResource.en.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void BtnInboud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryInbound = new InventoryChange("Inbound");
                inventoryInbound.Owner = this;
                //inventoryInbound.Left = this.Left + (this.Width - inventoryInbound.Width) / 2;
                //inventoryInbound.Top = this.Top + (this.Height - inventoryInbound.Height) / 2;
                inventoryInbound.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnOutboud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryOutbound = new InventoryChange("Outbound");
                inventoryOutbound.Owner = this;
                //inventoryOutbound.Left = this.Left + (this.Width - inventoryOutbound.Width) / 2;
                //inventoryOutbound.Top = this.Top + (this.Height - inventoryOutbound.Height) / 2;
                inventoryOutbound.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        //private void MenuItem_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }


    //public class ItemViewModel
    //{
    //    public int ItemId { get; set; }
    //    public string ItemName { get; set; }
    //    public string CategoryName { get; set; }
    //    public string Unit { get; set; }
    //    public string Location { get; set; }
    //    public string Description { get; set; }
    //    public int Quantity { get; set; }
    //    public decimal Total { get; set; }
    //    public DateTime SettleDate { get; set; }
    //    public int SettlementId { get; set; }
    //}
}
