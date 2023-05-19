using CarSalesSystem.Admin.Pages;
using CarSalesSystem.Model;
using CarSalesSystem.Viewmodel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
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
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.Admin.Windows
{
    /// <summary>
    /// Interaction logic for UpdateRankMoney.xaml
    /// </summary>
    public partial class UpdateRankMoney : Window
    {
        public UpdateRankMoney()
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
                maximumNotificationCount: MaximumNotificationCount.FromCount(3));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (typeComboBox.Text.Length == 0)
            {
                notifier.ShowWarning("Rank type is required.");
                return;
            }
            if(limitBox.Text.Length == 0)
            {
                notifier.ShowWarning("Cash limit is required.");
                return;
            }
            if(discountBox.Text.Length == 0)
            {
                notifier.ShowWarning("Discount is required.");
                return;
            }
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["NamConnection"].ConnectionString;
            try
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE RANK_MONEY
                        SET RANK_ID=@RANK_ID,RANK_TYPE=@RANK_TYPE,CASH_LIMIT=@CASH_LIMIT,DISCOUNT=@DISCOUNT
                        WHERE RANK_ID=@RANK_ID";
                    command.Parameters.AddWithValue("@RANK_ID", "R0" + typeComboBox.Text);
                    command.Parameters.AddWithValue("@RANK_TYPE", typeComboBox.Text);
                    command.Parameters.AddWithValue("@CASH_LIMIT",limitBox.Text);
                    command.Parameters.AddWithValue("@DISCOUNT", discountBox.Text);
                    command.ExecuteNonQuery();
                }
                notifier.ShowSuccess("Updated successfully");
            }
            catch (Exception exception)
            {
                notifier.ShowError(exception.Message);
            }
            finally
            {
                connection.Close();
            }

        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            if (e.Text.StartsWith("-"))
                notifier.ShowWarning("Negative value is not allowed!");
        }

        private void typeComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["NamConnection"].ConnectionString;
            try
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT *
                                FROM RANK_MONEY
                                WHERE RANK_TYPE=@RANK_TYPE";
                    command.Parameters.AddWithValue("@RANK_TYPE", typeComboBox.Text);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Console.WriteLine(String.Format("{0}", reader[0]));
                            limitBox.Text = reader[2].ToString();
                            discountBox.Text = reader[3].ToString();

                        }
                    }
                }
            }
            catch (Exception exception)
            {
                notifier.ShowError(exception.Message);
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
