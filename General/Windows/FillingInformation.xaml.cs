using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Threading;
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using System.Configuration;
using System.Text.RegularExpressions;
using CarSalesSystem.Viewmodel;

namespace CarSalesSystem.General.Windows
{
    /// <summary>
    /// Interaction logic for FillingInformation.xaml
    /// </summary>
    public partial class FillingInformation : Window
    {
        public String verifiedEmail = "";
        public FillingInformation()
        {
            InitializeComponent();
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


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

        private void usernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text.Equals("Enter username"))
                usernameTextBox.Text = "";
        }

        private void usernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text.Equals(""))
                usernameTextBox.Text = "Enter username";
        }
        private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            phPass.Visibility = Visibility.Hidden;
        }

        private void passwordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordTextBox.Password))
                phPass.Visibility = Visibility.Visible;
        }

        private void confirmTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            confirmPhPass.Visibility = Visibility.Hidden;
        }

        private void confirmTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(confirmTextBox.Password))
                confirmPhPass.Visibility = Visibility.Visible;
        }
        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text.Equals(" Enter username"))
            {
                //noti
                notifier.ShowWarning("Please enter your username!");
                usernameTextBox.Focus();
                return;
            }
            if (passwordTextBox.Password.Length == 0)
            {
                //noti
                notifier.ShowWarning("Please type in your password!"); ;
                passwordTextBox.Focus();
                return;
            }
            if (confirmTextBox.Password.Length == 0)
            {
                //noti
                notifier.ShowWarning("Please confirm your password!"); ;
                confirmTextBox.Focus();
                return;
            }
            if (passwordTextBox.Password != confirmTextBox.Password)
            {
                //noti
                notifier.ShowWarning("Your password does not match!"); ;
                passwordTextBox.Focus();
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
                        FROM ACCOUNT
                        WHERE USERNAME=@USERNAME";
                    command.Parameters.AddWithValue("@USERNAME", usernameTextBox.Text);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        notifier.ShowWarning("This username is already taken, please try something else.");
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
            try
                {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    /*command.CommandText = @"SELECT *
                        FROM ACCOUNT
                        WHERE USERNAME=@USERNAME";
                    command.Parameters.AddWithValue("@USERNAME", usernameTextBox.Text);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        notifier.ShowWarning("This username is already taken, please try something else.");
                        return;
                    }*/
                    
                    command.CommandText = @"INSERT INTO ACCOUNT(USERNAME,PASS,TYPE_USER)
                        VALUES (@USERNAME, @PASS, @TYPE_USER)";
                    command.Parameters.AddWithValue("@USERNAME", usernameTextBox.Text);
                    command.Parameters.AddWithValue("@PASS", passwordTextBox.Password);
                    command.Parameters.AddWithValue("@TYPE_USER", 2);
                    command.ExecuteNonQuery();
                    
                    command.CommandText = @"INSERT INTO CUSTOMER (CUS_NAME, CUS_ACCOUNT, PHONE, CUS_ADDRESS, REGIST_DATE, RANK_ID, CUS_EMAIL)
                        VALUES (@CUS_NAME, @CUS_ACCOUNT, @PHONE, @CUS_ADDRESS, @REGIST_DATE, @RANK_ID, @CUS_EMAIL)";
                    command.Parameters.AddWithValue("@CUS_NAME", nameBox.Text);
                    command.Parameters.AddWithValue("@CUS_ACCOUNT", usernameTextBox.Text);
                    command.Parameters.AddWithValue("@PHONE", phoneBox.Text);
                    command.Parameters.AddWithValue("@CUS_ADDRESS", addressBox.Text);
                    command.Parameters.AddWithValue("@REGIST_DATE", DateTime.Now.ToShortDateString());
                    command.Parameters.AddWithValue("@RANK_ID", "R00");
                    command.Parameters.AddWithValue("@CUS_EMAIL", verifiedEmail);
                    command.ExecuteNonQuery();
                    

                }
                SuccessfulMessage message = new SuccessfulMessage();
                message.Show();
                this.Close();

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
            /*
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Insert into ACCOUNT(USERNAME,PASS,TYPE_USER) values('" + usernameTextBox.Text + "','" + passwordTextBox.Password + "','" + "2')", connection);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //create new customer following new account registration
                cmd = new SqlCommand("INSERT INTO CUSTOMER (CUS_NAME, CUS_ACCOUNT, PHONE, CUS_ADDRESS, REGIST_DATE, IMG, RANK_ID) VALUES('" + nameBox.Text +
                                    "','" + usernameTextBox.Text +
                                    "','" + phoneBox.Text +
                                    "','" + addressBox.Text +
                                    "','" + DateTime.Now.ToShortDateString() +
                                    "', NULL,'R00')", connection) ;
                cmd.ExecuteNonQuery();
            }
            catch(Exception exception)
            {
                //noti
                notifier.ShowError(exception.Message);
                return;
            }
            connection.Close();*/
            
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addressBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (addressBox.Text.Equals("Address"))
                addressBox.Text = "";
        }

        private void addressBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (addressBox.Text.Equals(""))
                addressBox.Text = "Address";
        }

        private void nameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text.Equals("Full name"))
                nameBox.Text = "";
        }

        private void nameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text.Equals(""))
                nameBox.Text = "Full name";

        }

        private void phoneBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (phoneBox.Text.Equals("Phone number"))
                phoneBox.Text = "";

        }

        private void phoneBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (phoneBox.Text.Equals(""))
                phoneBox.Text = "Phone number";

        }
    }
}
