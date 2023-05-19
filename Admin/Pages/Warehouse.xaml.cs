using CarSalesSystem.Viewmodel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarSalesSystem.Admin.Pages
{
    /// <summary>
    /// Interaction logic for Warehouse.xaml
    /// </summary>
    public partial class WarehousePG : Page
    {
        public WarehousePG()
        {
            InitializeComponent();
            if(AccountInfo.Type_User == 1)
            {
                btnHistoryImport.Visibility = Visibility.Hidden;
            }
            else btnHistoryImport.Visibility = Visibility.Visible;
        }
    }
}
