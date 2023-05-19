using CarSalesSystem.General.Windows;
using CarSalesSystem.Viewmodel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.General
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }
        public string email = "";
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
        void CloseWindow(Type type)
        {
            var window = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.GetType() == type);
            if (window != null)
                window.Close();
        }
        private void emailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(email))
            {
                emailTextBox.Text = email;
                return;
            }
            if (emailTextBox.Text.Equals("Email") || emailTextBox.Text.Equals("Enter a valid email."))
                emailTextBox.Text = "";
        }

        private void emailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (emailTextBox.Text.Equals(""))
                emailTextBox.Text = "Email";
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            SignIn signIn = new SignIn();
            signIn.Show();
            CloseWindow(typeof(SignUp));
            

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(emailTextBox.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                emailTextBox.Text = "Enter a valid email.";
                emailTextBox.Select(0, emailTextBox.Text.Length);
                
                return;
            }

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["CarSalesSystem.Properties.Settings.CARSALESSYSTEMConnectionString"].ConnectionString;
            try
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT *
                        FROM CUSTOMER
                        WHERE CUS_EMAIL=@CUS_EMAIL";
                    command.Parameters.AddWithValue("@CUS_EMAIL", emailTextBox.Text);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        notifier.ShowWarning("This email has already been registered.");
                        return;
                    }

                }
            }
            catch (Exception exception)
            {
                notifier.ShowError(exception.Message);
                return;
            }
            finally
            {
                connection.Close();
            }
            email = emailTextBox.Text.ToString();
            WpfMessageBox wpfMessageBox = new WpfMessageBox();
            wpfMessageBox.Show();
            wpfMessageBox.storedEmail = emailTextBox.Text;
        }
        
 
    }
}
