using CarSalesSystem.Admin.Pages;
using CarSalesSystem.Customer.Windows;
using CarSalesSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CarSalesSystem.Customer.Pages
{
    /// <summary>
    /// Interaction logic for Bill.xaml
    /// </summary>
    public partial class Bill : Page
    {
        ObservableCollection<ORDERBILL> orderBills;
        CUSTOMER cus;
        public Bill(CUSTOMER _cus)
        {
            InitializeComponent();
            cus= _cus;
            datagridOrderBill.ItemsSource = DataProvider.Ins.DB.ORDERBILLs.Where(x=> x.CUSTOMER_ID == cus.CUS_ID).ToList();
        }

        private void datagridOrderBill_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ORDERBILL ordBill = datagridOrderBill.SelectedItem as ORDERBILL;
            if (ordBill != null)
            {
                MaintenanceBill maintainBillWindow = new MaintenanceBill(cus,ordBill);
                maintainBillWindow.Show();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text != "")
            {
                var filterList = DataProvider.Ins.DB.ORDERBILLs.Where(x => x.CUSTOMER.CUS_ID == cus.CUS_ID && x.PRODUCT.PRO_NAME.ToLower().Contains(txb.Text)).ToList();
                datagridOrderBill.ItemsSource = null;
                datagridOrderBill.ItemsSource = filterList;

            }
            else
            {
                datagridOrderBill.ItemsSource = DataProvider.Ins.DB.ORDERBILLs.Where( x => x.CUSTOMER_ID == cus.CUS_ID).ToList();
            }    
        }
    }
}
