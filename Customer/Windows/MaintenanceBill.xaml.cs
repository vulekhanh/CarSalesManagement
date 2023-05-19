using CarSalesSystem.Admin.Pages;
using CarSalesSystem.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
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
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.Customer.Windows
{
    /// <summary>
    /// Interaction logic for MaintenanceBill.xaml
    /// </summary>
    public partial class MaintenanceBill : Window
    {
        CUSTOMER customer1;
        ORDERBILL orderbill1;
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

        #region Constructor
        public MaintenanceBill()
        {
            InitializeComponent();
        }
        public MaintenanceBill(CUSTOMER _cus, ORDERBILL _ord)
        {
            InitializeComponent();
            customer1= _cus;
            orderbill1= _ord;
            txtTenKH.Text = customer1.CUS_NAME;
            decimal maintainPrice = CalculateMaintainFee(orderbill1);
            txtMaintainFee.Text = String.Format("{0:0,0}", maintainPrice);
            txtTenSP.Text = orderbill1.PRODUCT.PRO_NAME;
            txtMaintainDate.BlackoutDates.AddDatesInPast();
        }
        #endregion

        

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void btnMaintain_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["NamConnection"].ConnectionString;
            string query = "insert into MAINTENANCEBILL(MB_DATE,CUSTOMER_ID,PRO_ID,TOTALFEE,BILL_STATUS) values (@MB_DATE,@CUSTOMER_ID,@PRO_ID,@TOTALFEE,@BILL_STATUS)";
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MB_DATE", DateTime.Parse(txtMaintainDate.Text));
                    command.Parameters.AddWithValue("@CUSTOMER_ID", customer1.CUS_ID);
                    command.Parameters.AddWithValue("@PRO_ID", orderbill1.PRO_ID);
                    command.Parameters.AddWithValue("@TOTALFEE", decimal.Parse(txtMaintainFee.Text));
                    command.Parameters.AddWithValue("@BILL_STATUS", "UNPAID");
                    command.ExecuteNonQuery();
                    notifier.ShowSuccess("Successfully. Please bring your vehicle to our showroom in " + txtMaintainDate.Text);
                    this.Hide();
                }
                catch (Exception ex)
                {
                    notifier.ShowError("PLease choose a proper date");
                }
                connection.Close();
               
            
        }

        private decimal CalculateMaintainFee(ORDERBILL _ord)
        {
            float ins = (float)((_ord.PRODUCT.PRICE * 80) / 100);
            decimal price = _ord.PRODUCT.PRICE - (decimal)ins;
            return price;
        }
    }
}
