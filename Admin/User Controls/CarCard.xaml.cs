
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace CarSalesSystem.Admin.User_Controls
{
    /// <summary>
    /// Interaction logic for CarCard.xaml
    /// </summary>
    public partial class CarCard : UserControl
    {
        public CarCard()
        {
            InitializeComponent();
        }

        public string CarCompany
        {
            get { return (string)GetValue(CarCompanyProperty); }
            set { SetValue(CarCompanyProperty, value); }
        }
        public static readonly DependencyProperty CarCompanyProperty
            = DependencyProperty.Register("CarCompany", typeof(string), typeof(CarCard));

        public string CarName
        {
            get { return (string)GetValue(CarNameProperty); }
            set { SetValue(CarNameProperty, value); }
        }
        public static readonly DependencyProperty CarNameProperty
            = DependencyProperty.Register("CarName", typeof(string), typeof(CarCard));


        public string SaleNumber
        {
            get { return (string)GetValue(SaleNumberProperty); }
            set { SetValue(SaleNumberProperty, value); }
        }

        public static readonly DependencyProperty SaleNumberProperty
            = DependencyProperty.Register("SaleNumber", typeof(string), typeof(CarCard));


        public string Profit
        {
            get { return (string)GetValue(ProfitProperty); }
            set { SetValue(ProfitProperty, value); }
        }
        public static readonly DependencyProperty ProfitProperty 
            = DependencyProperty.Register("Profit", typeof(string), typeof(CarCard));
        
        //Active
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty 
            = DependencyProperty.Register("IsActive", typeof(bool), typeof(CarCard));
        
        // Image
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register("Image", typeof(ImageSource), typeof(CarCard));
    }
}
