using CarSalesSystem.General.Windows;
using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xaml;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.General
{
    /// <summary>
    /// Interaction logic for WpfMessageBox.xaml
    /// </summary>
    public partial class WpfMessageBox : Window
    {
        public String storedEmail = "";
        public WpfMessageBox()
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
        void HideWindow(Type type)
        {
            var window = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.GetType() == type);
            if (window != null)
                window.Hide();
        }
        protected MessageBoxResult _result = MessageBoxResult.No;
        private bool OTPvalidattion = false;
        void CloseWindow(Type type)
        {
            var window = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.GetType() == type);
            if (window != null)
                window.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnYes)
                _result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                _result = MessageBoxResult.No;
            OTPConfirmation confirmation = new OTPConfirmation();
            if(_result == MessageBoxResult.Yes)
            {
                
                CloseWindow(typeof(WpfMessageBox));
                //Send OTP code to email
                Random rand = new Random(); 
                String randomCode = (rand.Next(999999)).ToString();
                MailMessage message = new MailMessage();
                String to = storedEmail.ToString();
                String from = "20520215@gm.uit.edu.vn";
                String password = "kbagdlagxhmoevvl";
                String messageBody = "Your OTP code is: " + randomCode;
                message.To.Add(new MailAddress(to));
                message.From = new MailAddress(from);
                message.Body= messageBody;
                message.Subject = "Registration OTP code";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(from,password);
                try
                {
                    smtp.Send(message);
                    //noti
                    confirmation.storedCode= randomCode;
                    confirmation.storedEmail = this.storedEmail;
                    confirmation.Show();
                    confirmation.showMessage();
                    HideWindow(typeof(SignUp));
                }
                catch(Exception ex)
                {
                    notifier.ShowError("Cannot send OTP code to email");
                }
                
            }
            else
            {
                this.Close();
            }
        }
       

    }
}
