using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace CarSalesSystem.General.Windows
{
    /// <summary>
    /// Interaction logic for OTPConfirmation.xaml
    /// </summary>
    public partial class OTPConfirmation : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private int time = 180;
        private DispatcherTimer Timer;
        private int retryTimes = 3;
        public string storedCode;
        public String storedEmail ="";
        //Notification box
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
        public OTPConfirmation()
        {
            InitializeComponent();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }
        void Timer_Tick(object sender, EventArgs e)
        {
            if (time > 0)
            {
                if (time <= 10)
                {
                    TBCountDown.Foreground = Brushes.Red;
                }
                time--;
                TBCountDown.Text = string.Format("{0}:{1}", time / 60, time % 60);

            }
            else
                Timer.Stop();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            retryTimes--;
            if(retryTimes == 0)
            {
                notifier.ShowWarning("You have requested OTP code too many times!");
                Timer.Stop();
                this.Close();
                return;
            }
            time = 180;

            //Send OTP code to email
            SignUp signUp= new SignUp();
            signUp.Show();
            this.Hide();
        }

        private void ConnectButton_TextChanged(object sender, TextChangedEventArgs e)
        {

            string typedCode = CodeDigit1.Text + CodeDigit2.Text + CodeDigit3.Text + CodeDigit4.Text + CodeDigit5.Text + ConnectButton.Text;
            if(!typedCode.Equals(storedCode))
            {
                notifier.ShowWarning("Invalid OTP code, please try again.");
                return;
            }
            FillingInformation fillingInformation = new FillingInformation();
            fillingInformation.verifiedEmail = this.storedEmail;
            fillingInformation.Show();
            this.Close();
        }
        
        public void showMessage()
        {
            notifier.ShowSuccess("Code successfully sent to email.");
        }
    }
}
