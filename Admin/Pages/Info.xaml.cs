using CarSalesSystem.Admin.Windows;
using CarSalesSystem.Model;
using CarSalesSystem.Viewmodel;
using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.Admin.Pages
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Page
    {
        EMPLOYEE employee;
        private string imagefilename;

        public Info(/*EMPLOYEE _employee*/)
        {
            InitializeComponent();          

            var empinfo = DataProvider.Ins.DB.EMPLOYEEs.Find(AccountInfo.IdAccount);
            if (empinfo.EMP_NAME != null)
                nameTextBox.Text = empinfo.EMP_NAME;
            if (empinfo.EMP_ADDRESS != null)
                addressTextBox.Text = empinfo.EMP_ADDRESS;
            if (empinfo.PHONE != null)
                phoneTextBox.Text = empinfo.PHONE;
            if (empinfo.GENDER != null)
                genderBox.Text = empinfo.GENDER;
            if (empinfo.EMP_DATE_OF_BIRTH != null)
                birthdayTextBox.SelectedDate = empinfo.EMP_DATE_OF_BIRTH;
            if (empinfo.IMG != null)
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = Converter.Instance.ToImage(empinfo.IMG);
                btnSelectImage.Background= imageBrush;
            }
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
                maximumNotificationCount: MaximumNotificationCount.FromCount(3));

            cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
        });

        private void informationButton_Click(object sender, RoutedEventArgs e)
        {
            
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["NamConnection"].ConnectionString;
            try
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    
                    command.CommandText = @"UPDATE EMPLOYEE
                        SET EMP_NAME=@EMP_NAME, GENDER=@GENDER, EMP_ADDRESS=@EMP_ADDRESS, EMP_DATE_OF_BIRTH=@EMP_DATE_OF_BIRTH,
                            PHONE=@PHONE
                        WHERE EMP_ID=@EMP_ID";
                    command.Parameters.AddWithValue("@EMP_ID", AccountInfo.IdAccount);
                    command.Parameters.AddWithValue("@EMP_NAME", nameTextBox.Text);
                    command.Parameters.AddWithValue("@GENDER", genderBox.Text);
                    command.Parameters.AddWithValue("@EMP_ADDRESS", addressTextBox.Text);
                    command.Parameters.AddWithValue("@EMP_DATE_OF_BIRTH", birthdayTextBox.SelectedDate.Value.ToString("MM-dd-yyyy"));
                    command.Parameters.AddWithValue("@PHONE", phoneTextBox.Text);
                    command.ExecuteNonQuery();
                }
                notifier.ShowSuccess("Successfully Updated Information");
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


        private void passwordButton_Click(object sender, RoutedEventArgs e)
        {
            if (newPassBox.SecurePassword.Length == 0)
            {
                notifier.ShowWarning("You must type in your new password.");
                return;
            }
            if (verifyPassBox.SecurePassword.Length == 0)
            {
                notifier.ShowWarning("You must verify your password.");
                return;
            }
            if (verifyPassBox.Password != newPassBox.Password)
            {
                notifier.ShowWarning("Please check your verify password again.");
                return;
            }
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["NamConnection"].ConnectionString;
            try
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT PASS
                        FROM ACCOUNT
                        WHERE USERNAME=@USERNAME";
                    command.Parameters.AddWithValue("@USERNAME", AccountInfo.Username);
                    String password = command.ExecuteScalar().ToString();
                    if (!oldPassBox.Password.Equals(password))
                    {
                        notifier.ShowWarning("Your old password is incorrect, please try again!");
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                notifier.ShowError(exception.Message);
            }
            finally { connection.Close(); }

            try
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ACCOUNT
                        SET PASS=@PASS
                        WHERE USERNAME=@USERNAME";
                    
                    command.Parameters.AddWithValue("@PASS", newPassBox.Password);
                    command.Parameters.AddWithValue("@USERNAME", AccountInfo.Username);
                    command.ExecuteNonQuery();
                }
                notifier.ShowSuccess("Successfully Updated New Password");
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

        private void rankUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateRankMoney updateRankMoney = new UpdateRankMoney();
            updateRankMoney.Show();
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == DialogResult.OK)
            {
                imagefilename = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagefilename);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                btnSelectImage.Background = imageBrush;
                var empinfo = DataProvider.Ins.DB.EMPLOYEEs.Find(AccountInfo.IdAccount);
                empinfo.IMG = empinfo.IMG = Converter.Instance.StreamFile(imagefilename);
                DataProvider.Ins.DB.SaveChanges();
                notifier.ShowSuccess("Successfully Updated New Avatar");
            }
        }
    }
}
