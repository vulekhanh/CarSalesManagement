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

namespace CarSalesSystem.General.Windows
{
    /// <summary>
    /// Interaction logic for SuccessfulMessage.xaml
    /// </summary>
    public partial class SuccessfulMessage : Window
    {
        public SuccessfulMessage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SignIn signIn= new SignIn();
            signIn.Show();
            this.Close();
        }
    }
}
