using CarSalesSystem.Admin.Pages;
using CarSalesSystem.Customer.Windows;
using CarSalesSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace CarSalesSystem.Customer.Pages
{
    /// <summary>
    /// Interaction logic for Store.xaml
    /// </summary>
    public partial class Store : Page, INotifyPropertyChanged
    {
        ObservableCollection<PRODUCT> products;
        CUSTOMER customer;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }    
        }
        #region Notifier
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
        #endregion

        public Store(CUSTOMER cus)
        {
            InitializeComponent();
            customer = cus;
            LoadDataIntoCombobox(cbProducer);
            Thread thread = new Thread(delegate ()
            {
                // Get và xác định số trang
                var db = new CARSALESSYSTEMEntities();
                int numPages = (int)Math.Ceiling(db.PRODUCTs.Count() / 10.0);
                if (numPages == 0) numPages = 1;

                // Đưa vào Combobox
                List<string> pageNumber = new List<string>();
                for (int i = 1; i <= numPages; i++)
                {
                    pageNumber.Add(i + "/" + numPages);
                }
                Dispatcher.Invoke(() =>
                {
                    cbPage.ItemsSource = pageNumber;
                    cbPage.SelectedIndex = 0;
                });

                // Lấy danh sách sản phẩm
                products = new ObservableCollection<PRODUCT>(db.PRODUCTs);
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = i; j < products.Count; j++)
                    {
                        if (compare_sort(products[i], products[j]))
                        {
                            PRODUCT temp = products[i];
                            products[i] = products[j];
                            products[j] = temp;
                        }
                    }
                }
                // Cập nhật UI
                Dispatcher.Invoke(() =>
                {
                    listProduct.ItemsSource = products.Skip(cbPage.SelectedIndex * 10).Take(10);
                });
            });
            thread.Start();
        }

        private bool compare_sort(PRODUCT pRODUCT1, PRODUCT pRODUCT2)
        {
            int sortType = 0;
            Dispatcher.Invoke(() => { sortType = cbSort.SelectedIndex; });
            switch (sortType)
            {
                case 1: return pRODUCT1.PRICE < pRODUCT2.PRICE; // Tăng dần giá cả
                case 2: return pRODUCT1.PRICE > pRODUCT2.PRICE; // Giảm dần giá cả
                default: return true;
            }
        }

        private void ListProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PRODUCT prd = listProduct.SelectedItem as PRODUCT;
            DetailCar detailCar = new DetailCar(prd, customer);
            detailCar.Show();
        }


        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Tạo mới danh sách sản phẩm có tên chứa nội dung ô tìm kiếm
            ObservableCollection<PRODUCT> searchProducts = new ObservableCollection<PRODUCT>();
            // Nếu ô tìm kiếm rỗng, thì lấy tất cả sản phẩm
            if (txbSearch.Text.Length == 0)
            {
                refresh(true);
            }

            // Nếu ô tìm kiếm có nội dung
            else
            {
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i].PRO_NAME.ToUpper().Contains(txbSearch.Text.ToUpper())) // Nếu tìm thấy tên phù hợp
                    {
                        searchProducts.Add(products[i]); // Thì thêm vào danh sách mới
                    }
                }

                // Nếu tìm thấy ít nhất 1 sản phẩm thì hiển thị, không thì thông báo
                if (searchProducts.Count > 0)
                {
                    reset_page(searchProducts);
                    listProduct.Visibility = Visibility.Visible;
                    listProduct.ItemsSource = searchProducts;

                }
                else
                {
                    listProduct.Visibility = Visibility.Hidden;
                }
            }
        }

        #region reset and refresh list
        private void reset_page(ObservableCollection<PRODUCT> searchProducts)
        {
            List<string> pageNumber = new List<string>();
            int numPages = (int)Math.Ceiling(products.Count() / 10.0);
            if (numPages == 0) numPages = 1;

            for (int i = 1; i <= numPages; i++)
            {
                pageNumber.Add(i + "/" + numPages);
            }
            Dispatcher.Invoke(() =>
            {
                cbPage.ItemsSource = pageNumber;
                cbPage.SelectedIndex = 0;
            });
        }

        public void refresh(bool Data)
        {
            if (!Data) return;


            // Nếu lượng sản phẩm thêm vào nhiều và tạo thành trang mới
            int curPage = cbPage.SelectedIndex;
            int newNumPages = (int)Math.Ceiling(products.Count / 10.0);
            List<string> pageNumber = new List<string>();
            for (int j = 1; j <= newNumPages; j++)
            {
                pageNumber.Add(j + "/" + newNumPages);
            }
            cbPage.ItemsSource = pageNumber;
            cbPage.SelectedIndex = curPage;
            listProduct.ItemsSource = products.Skip(cbPage.SelectedIndex * 10).Take(10);
        }
        #endregion

        #region combobox producer effect
        private void cbProducer_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Background = Brushes.Transparent;
        }

        private void cbProducer_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Background = Brushes.LightGray;
        }
        #endregion

        #region Turn page
        private void btLeft_Click(object sender, RoutedEventArgs e)
        {
            if (products != null)
            {
                if (cbPage.SelectedIndex > 0)
                {
                    listProduct.ItemsSource = products.Skip(--cbPage.SelectedIndex * 10).Take(10);
                }
            }
        }

        private void btRight_Click(object sender, RoutedEventArgs e)
        {
            if (products != null)
            {
                if (cbPage.SelectedIndex < cbPage.Items.Count - 1)
                {
                    listProduct.ItemsSource = products.Skip(++cbPage.SelectedIndex * 10).Take(10);
                }
            }
        }
        #endregion

        private void LoadDataIntoCombobox(ComboBox comboBox)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["CarSalesSystem.Properties.Settings.CARSALESSYSTEMConnectionString"].ConnectionString;
            string query = "select PRODUCER.PRODUCER_ID, PRODUCER.PRODUCER_NAME from PRODUCER";
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = command;
                DataTable table1 = new DataTable();
                da.Fill(table1);

                DataRow itemRow = table1.NewRow();
                itemRow[1] = "ALL";
                table1.Rows.InsertAt(itemRow, 0);

                comboBox.ItemsSource = table1.AsDataView();
                comboBox.DisplayMemberPath = "PRODUCER_NAME";
                comboBox.SelectedValuePath = "PRODUCER_ID";

            }
            catch (Exception ex)
            {
                notifier.ShowError(ex.Message);
            }
            connection.Close();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (products != null)
            {
                // Sắp xếp lại
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = i; j < products.Count; j++)
                    {
                        if (compare_sort(products[i], products[j]))
                        {
                            PRODUCT temp = products[i];
                            products[i] = products[j];
                            products[j] = temp;
                        }
                    }
                }

                listProduct.ItemsSource = products.Skip(cbPage.SelectedIndex * 10).Take(10);
            }
        }

        private void cbProducer_LostFocus(object sender, RoutedEventArgs e)
        {
            // Tạo mới danh sách sản phẩm có tên chứa loại sản phẩm cần lọc
            ObservableCollection<PRODUCT> filterProducts = new ObservableCollection<PRODUCT>();
            if (cbProducer.SelectedIndex == -1)
            {
                reset_page(products);
                listProduct.ItemsSource = products;
            }
            else
            {
                if (cbProducer.SelectedIndex == 0)
                {
                    reset_page(products);
                    listProduct.ItemsSource = products;
                }
                else
                {
                    
                    for (int i = 0; i < products.Count; i++)
                    {
                        if (products[i].PRODUCER.PRODUCER_NAME == cbProducer.Text.ToString()) // Nếu tìm thấy tên phù hợp
                        {
                            filterProducts.Add(products[i]); // Thì thêm vào danh sách mới

                        }
                    }
                    reset_page(filterProducts);
                    listProduct.ItemsSource = filterProducts;
                }
            }
        }
    }
}
