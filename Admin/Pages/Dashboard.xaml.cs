using CarSalesSystem.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarSalesSystem.Admin.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page, INotifyPropertyChanged
    {
        PRODUCT product;
        

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SeriesCollection seriesCollection { get; set; }
        public string[] labels { get; set; }
        public Func<string, string> formatter { get; set; }
        SELLBILL[] sellBills = DataProvider.Ins.DB.SELLBILLs.ToArray();
       
        public Dashboard()
        {
            InitializeComponent();
            #region Revenue Chart
            ChartValues<decimal> values = new ChartValues<decimal>();
            foreach (var price in sellBills)
            {
                values.Add((decimal)price.TOTAL_PRICE);
            }    
             
            seriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Title = "Revenue",
                    PointGeometrySize = 10,
                    StrokeThickness = 2,
                    Values = values
                }
            };
            DataContext = this;
            labels = new[] { "1","2","3","4","5","6","7","8","9","10","11","12" };
            #endregion

            #region Information Of CarCard
            cardBugatti.SaleNumber = NumberCarSale("P001").ToString();
            cardBugatti.Profit = String.Format("{0:0,0}", Calculateprofit("P001"));

            cardMaserati.SaleNumber = NumberCarSale("P002").ToString();
            cardMaserati.Profit = String.Format("{0:0,0}", Calculateprofit("P002"));

            cardToyota.SaleNumber = NumberCarSale("P003").ToString();
            cardToyota.Profit = String.Format("{0:0,0}", Calculateprofit("P003"));

            cardTesla.SaleNumber = NumberCarSale("P004").ToString();
            cardTesla.Profit = String.Format("{0:0,0}", Calculateprofit("P004"));

            cardLexus.SaleNumber = NumberCarSale("P006").ToString();
            cardLexus.Profit = String.Format("{0:0,0}", Calculateprofit("P006"));
            #endregion

            #region Information Of InfoCard
            salesCard.Number = DataProvider.Ins.DB.SELLBILLs.Count().ToString();
            orderCard.Number = DataProvider.Ins.DB.ORDERBILLs.Count().ToString();
            var revenueInfo  = DataProvider.Ins.DB.SELLBILLs.Sum(x => x.TOTAL_PRICE);
            revenueCard.Number = String.Format("{0:0,0}", revenueInfo);
            #endregion
        }

        private void swap(SELLBILL a, SELLBILL b)
        {
            SELLBILL temp = a;
            a = b;
            b = temp;
        }
        private int NumberCarSale(string carID)
        {
            return DataProvider.Ins.DB.SELLBILLs.Where(x => x.PRO_ID == carID && x.SB_DATEB <= DateTime.Now).Count();
        }

        private decimal Calculateprofit(string carID)
        {
            product = DataProvider.Ins.DB.PRODUCTs.Where(x => x.PRO_ID.Equals(carID)).FirstOrDefault();
            decimal revenue = product.PRICE*NumberCarSale(product.PRO_ID);
            return revenue + (revenue*30)/100;
        }

    }
}
