using LiveCharts;
using LiveCharts.Wpf;
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
using System.Windows.Shapes;
using static WareMaster.Analyse;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for Analyse.xaml
    /// </summary>
    public partial class Analyse : Window
    {
        private List<InventoryData> allRecords;
        private List<CategoryItem> categoryItems;

        public Analyse()
        {
            InitializeComponent();
            InitailizeInventoryChart();
            InitializeCategoryPie();
           

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
                    //GridContent.Height = 920;
                    IsMaximized = true;
                }
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void InitailizeInventoryChart()
        {
            // get all inbound record of last month
            var inboundSummary = Globals.wareMasterEntities.Transactions
                            .Where(transaction => transaction.Transaction_Date.Month == 11 && transaction.Quantity > 0)
                            .GroupBy(transaction => transaction.Transaction_Date.Day)
                             .Select(group => new inventorySummary
                             {
                                 Date = group.Key,
                                 Record = group.Count()
                             })
                            .ToList();

            List<int> inboundList = inboundSummary.Select(summary => summary.Record).ToList();

            foreach (var record in inboundList)
            {
                Console.WriteLine(record);
            }

            string formattedValues = string.Join(",", inboundList);

            var outboundSummary = Globals.wareMasterEntities.Transactions
                            .Where(transaction => transaction.Transaction_Date.Month == 11 && transaction.Quantity < 0)
                            .GroupBy(transaction => transaction.Transaction_Date.Day)
                             .Select(group => new inventorySummary
                             {
                                 Date = group.Key,
                                 Record = group.Count()
                             })
                            .ToList();

            List<int> outboundList = outboundSummary.Select(summary => summary.Record).ToList();

            foreach (var record in outboundList)
            {
                Console.WriteLine(record);
            }

        }
        private class inventorySummary
        {
            public int Date { get; set; }
            public int Record { get; set; }
            public override string ToString()
            {
                return $"Date: {Date}, Record: {Record}";
            }
        }

        public class CategoryItem
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int TotalItems { get; set; }
        }

        private void InitializeCategoryPie()
        {
            var query = from category in Globals.wareMasterEntities.Categories
                        join item in Globals.wareMasterEntities.Items
            on category.id equals item.Category_Id into CategoryItems
                        select new CategoryItem
                        {
                            CategoryId = category.id,
                            CategoryName = category.Category_Name,
                            TotalItems = CategoryItems.Count()
                        };

            var categoryItems = query.ToList();
            PieSeriesCollection = new SeriesCollection(); 

            foreach (var categoryItem in categoryItems)
            {
                PieSeriesCollection.Add(new PieSeries
                {
                    Title = categoryItem.CategoryName,
                    Values = new ChartValues<double> { categoryItem.TotalItems },
                    DataLabels = true
                });
            }

            DataContext = this;
           
        }

        public SeriesCollection PieSeriesCollection { get; set; }

    }

}
