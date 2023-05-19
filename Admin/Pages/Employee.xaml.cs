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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarSalesSystem.Admin.Pages
{
    /// <summary>
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class EmployeePG : Page
    {
        public EmployeePG()
        {
            InitializeComponent();
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Addemp addemp = new Addemp();
            int k = DataProvider.Ins.DB.EMPLOYEEs.Count()+1;
            string s = "";
            if (k < 10)
            {
                s = "EP0" + k.ToString();
            }
            else
            {
                s = "EP" + k.ToString();
            }
            addemp.idBox.Text = s;
            addemp.dowBox.Text = DateTime.UtcNow.Date.ToString();
            addemp.Show();
            
        }
    }
}
