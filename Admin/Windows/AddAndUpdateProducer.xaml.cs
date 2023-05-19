using CarSalesSystem.Model;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.Admin.Windows
{
    /// <summary>
    /// Interaction logic for AddAndUpdateProducer.xaml
    /// </summary>
    public partial class AddAndUpdateProducer : Window
    {
        public AddAndUpdateProducer()
        {
            InitializeComponent();
        }
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: System.Windows.Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
        });

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbIdProducer.SelectedIndex != 0)
            {
                var typeItem = (String)cbIdProducer.SelectedValue;
                var s = DataProvider.Ins.DB.PRODUCERs.Find(typeItem);
                tbProducer.Text = s.PRODUCER_NAME;
            }
            else
            {
                tbProducer.Text = "";
            }
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(cbIdProducer.SelectedIndex == 0)
            {
                var listproducer = DataProvider.Ins.DB.PRODUCERs.ToList();
                PRODUCER producer = new PRODUCER();
                producer.PRODUCER_ID="CC"+(listproducer.Count+1);
                producer.PRODUCER_NAME = tbProducer.Text;
                DataProvider.Ins.DB.PRODUCERs.Add(producer);
            }
            else
            {
                var item = DataProvider.Ins.DB.PRODUCERs.Find(cbIdProducer.SelectionBoxItem.ToString());
                item.PRODUCER_NAME = tbProducer.Text;
            }
            notifier.ShowSuccess("Update Successfully");
            DataProvider.Ins.DB.SaveChanges();
            this.Close();
        }
    }
}
