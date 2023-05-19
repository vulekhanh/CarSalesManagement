using CarSalesSystem.Model;
using CarSalesSystem.Viewmodel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
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
using ToastNotifications;
using CarSalesSystem.General;
using System.Drawing;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Configuration;

namespace CarSalesSystem.Customer.Windows
{
    /// <summary>
    /// Interaction logic for OrderBill.xaml
    /// </summary>
    public partial class OrderBill : Window
    {
        CUSTOMER customer1;
        PRODUCT product1;
        
        public OrderBill()
        {
            InitializeComponent();
        }
        public OrderBill(PRODUCT _product, CUSTOMER _customer)
        {
            InitializeComponent();
            customer1 = _customer;
            product1 = _product;
            txtTenSP.Text = product1.PRO_NAME;
            txtTenKH.Text = customer1.CUS_NAME;
            txtNgayDatHang.SelectedDate = DateTime.Now;
            txtDiscount.Text = customer1.RANK_MONEY.DISCOUNT.ToString() + "%";
            decimal cashSale = CalculateCashSale(customer1, product1);
            txtCashSale.Text = String.Format("{0:0,0}",cashSale);
            decimal totalPrice = CalcualteTotalPrice(customer1, product1);
            txtPrice.Text = String.Format("{0:0,0}", totalPrice);
            PRODUCT P = DataProvider.Ins.DB.PRODUCTs.Where(x => x.PRO_ID == _product.PRO_ID).First();
            if (P.IMG != null)
            {
                Stream StreamObj = new MemoryStream(P.IMG);
                BitmapImage BitObj = new BitmapImage();
                BitObj.BeginInit();
                BitObj.StreamSource = StreamObj;
                BitObj.EndInit();
                imgCarOrder.Source = BitObj;
            }
           
            btnOrder.Visibility = Visibility.Visible;

        }
        #region Notifier
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });
        #endregion

        #region Calculate
        private decimal CalcualteTotalPrice(CUSTOMER cus, PRODUCT pro)
        {
            return pro.PRICE - decimal.Parse(txtCashSale.Text);
        }
        private decimal CalculateCashSale(CUSTOMER cus,PRODUCT pro)
        {
            float discount = (float)cus.RANK_MONEY.DISCOUNT / 100;
            decimal totalPrice = pro.PRICE * (decimal)discount * 1;
            if (cus.RANK_MONEY.CASH_LIMIT < totalPrice)
            {
                return (decimal)cus.RANK_MONEY.CASH_LIMIT;
            }
            else return totalPrice;
        }
        #endregion

        #region Event Handler

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["NamConnection"].ConnectionString;
            string query = "insert into ORDERBILL(OB_DATEB,CUSTOMER_ID,PRO_ID,QUANTITY,RANK_ID,TOTAL_PRICE) values (@OB_DATED,@CUS_ID,@PRO_ID,@QUANTITY,@RANK_ID,@TOTAL_PRICE)";
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OB_DATED", DateTime.Parse(txtNgayDatHang.Text));
                command.Parameters.AddWithValue("@CUS_ID", customer1.CUS_ID);
                command.Parameters.AddWithValue("@PRO_ID", product1.PRO_ID);
                command.Parameters.AddWithValue("@QUANTITY", 1);
                command.Parameters.AddWithValue("@RANK_ID", customer1.RANK_ID);
                command.Parameters.AddWithValue("@TOTAL_PRICE", decimal.Parse(txtPrice.Text));
                command.ExecuteNonQuery();
                notifier.ShowSuccess("Successfully");
            }
            catch (Exception ex)
            {
                notifier.ShowError(ex.Message);
            }
            connection.Close();
        }
        #endregion
    }
}
