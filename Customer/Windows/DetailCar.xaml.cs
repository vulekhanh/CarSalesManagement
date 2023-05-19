using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CarSalesSystem.Model;

namespace CarSalesSystem.Customer.Windows
{
    /// <summary>
    /// Interaction logic for DetailCar.xaml
    /// </summary>
    public partial class DetailCar : Window
    {
        decimal l;
        PRODUCT  product;
        CUSTOMER khachhang;
        public DetailCar(PRODUCT car, CUSTOMER _khachhang)
        {
            InitializeComponent();
            product = car;
            khachhang = _khachhang;
            lbProducer.Content = car.PRODUCER.PRODUCER_NAME;
            lbProductName.Content = car.PRO_NAME;
            lbEngineLayout.Content = car.ENGINELAYOUT;
            lbDisplacement.Content = car.DISPLACEMENT;
            lbAcceleration.Content = car.ACCELERATION;
            lbMaxSpeed.Content = car.MAXSPEED;
            lbMaxPower.Content = car.MAXPOWER;
            lbTraction.Content = car.TRACTION;
            lbPrice.Content = String.Format("{0:0,0}", car.PRICE);
            PRODUCT P = DataProvider.Ins.DB.PRODUCTs.Where(x => x.PRO_ID == car.PRO_ID).First();
            if (P.IMG != null)
            {
                Stream StreamObj = new MemoryStream(P.IMG);
                BitmapImage BitObj = new BitmapImage();
                BitObj.BeginInit();
                BitObj.StreamSource = StreamObj;
                BitObj.EndInit();
                imgCar.Source = BitObj;
            }

        }



        private void btnBooking_Click(object sender, RoutedEventArgs e)
        {

            OrderBill orderBill = new OrderBill(product, khachhang);
            orderBill.Show();
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
