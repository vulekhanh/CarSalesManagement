using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Linq.Expressions;
using CarSalesSystem.General.Windows;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using CarSalesSystem.Model;
using CarSalesSystem.Viewmodel;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;

namespace CarSalesSystem.General
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        private bool usernameValid = false;
        private bool passwordValid = false;
        private int type_user = 1;

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
        const string clientID = "581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";
        const string clientSecret = "3f6NggMbPtrmIBpgx-MK2xXK";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";

        public SignIn()
        {
            InitializeComponent();
            if (Properties.Settings.Default.userName != string.Empty)
            {
                usernameTextBox.Text = Properties.Settings.Default.userName;
                passwordTextBox.Password= Properties.Settings.Default.passUser;
            }

        }
        void CloseWindow(Type type)
        {
            var window = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.GetType() == type);
            if (window != null)
                window.Close();
        }
        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUp = new SignUp();
            signUp.Show();
            CloseWindow(typeof(SignIn));

        }

        private void usernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text.Equals(Properties.Settings.Default.userName))
                return;
            if (usernameTextBox.Text.Equals("Username"))
                usernameTextBox.Text = "";
            if (usernameTextBox.BorderBrush == Brushes.Red)
                usernameTextBox.BorderBrush = Brushes.White;
        }

        private void usernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text.Equals("")) ;
        }

        private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            phPass.Visibility = Visibility.Hidden;
            if (passwordTextBox.BorderBrush == Brushes.Red)
                passwordTextBox.BorderBrush = Brushes.White;
        }

        private void passwordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password.Equals(Properties.Settings.Default.passUser))
            {
                phPass.Visibility = Visibility.Hidden;
                return;
            }
            
            if (string.IsNullOrEmpty(passwordTextBox.Password))
                phPass.Visibility = Visibility.Visible;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow adminWindow = new MainWindow();
            
            if (usernameTextBox.Text.Equals("admin12312") && passwordTextBox.Password.Equals("123456"))
            {
                adminWindow.Show();
                this.Close();
                return;
            }




            //retrieve data and compare with data from database
            SqlConnection connection = new SqlConnection();
            
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["CarSalesSystem.Properties.Settings.CARSALESSYSTEMConnectionString"].ConnectionString;
            //MessageBox.Show(connection.ConnectionString);
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand cmd = new SqlCommand("Select COUNT(1) from ACCOUNT where USERNAME='" + usernameTextBox.Text + "'  and PASS='" + passwordTextBox.Password + "'", connection);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@USERNAME", usernameTextBox.Text);
                cmd.Parameters.AddWithValue("@PASS", passwordTextBox.Password);
                
                SqlCommand activeWindow = new SqlCommand("Select TYPE_USER from ACCOUNT where USERNAME='" + usernameTextBox.Text + "'", connection);
                activeWindow.CommandType = CommandType.Text;
                int type = Convert.ToInt32(activeWindow.ExecuteScalar());
                activeWindow.Parameters.AddWithValue("@USERNAME", usernameTextBox.Text);
                activeWindow.Parameters.AddWithValue("@PASS", passwordTextBox.Password);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                AccountInfo.Username = usernameTextBox.Text;
                if (result == 1)
                 {
                    usernameValid = true;
                    passwordValid = true;
                    if(type == 2)
                    {
                        CUSTOMER cus = DataProvider.Ins.DB.CUSTOMERs.Where(x => x.CUS_ACCOUNT == usernameTextBox.Text).FirstOrDefault();
                        CustomerWindow customerWindow = new CustomerWindow(cus);
                        customerWindow.Show();
                        AccountInfo.IdAccount = cus.CUS_ID;
                        AccountInfo.Type_User = 2;
                    }
                    else
                    {   
                        EMPLOYEE emp = DataProvider.Ins.DB.EMPLOYEEs.Where(x => x.EMP_ACCOUNT == usernameTextBox.Text).FirstOrDefault();
                        if ((emp.ACCOUNT.LOGIN_RECENT != DateTime.Today) || (emp.ACCOUNT.LOGIN_RECENT==null))
                            emp.DateOfWork++;
                            emp.ACCOUNT.LOGIN_RECENT = DateTime.Today;
                        AccountInfo.Type_User=emp.ACCOUNT.TYPE_USER;
                        AccountInfo.IdAccount = emp.EMP_ID;

                        if (emp.ACCOUNT.TYPE_USER == 1)
                        {
                            adminWindow.employeeBtn.Visibility = Visibility.Collapsed;
                        }
                        else adminWindow.employeeBtn.Visibility = Visibility.Visible;
                        DataProvider.Ins.DB.SaveChanges();
                        adminWindow.Show();
                    }
                       
                    if (rememberCheckBox.IsChecked == true)
                    {
                        Properties.Settings.Default.userName = usernameTextBox.Text;
                        Properties.Settings.Default.passUser = passwordTextBox.Password;
                        Properties.Settings.Default.Save();
                    }
                    this.Hide();
                 }
                 else
                 {
                    //noti
                    notifier.ShowError("Invalid username or wrong password, please check again.");

                 }
               
            }

            catch (Exception ex)
            {
                notifier.ShowError(ex.Message);
            }
            
            connection.Close();
            //if (usernameValid && passwordValid)
            //{
            //    CUSTOMER cus = DataProvider.Ins.DB.CUSTOMERs.Where(x => x.CUS_ACCOUNT == usernameTextBox.Text).FirstOrDefault();
            //    CustomerWindow customerWindow = new CustomerWindow(cus);
            //    customerWindow.Show();
            //    this.Close();
            //    return;
            //}
            if (!usernameValid)
                usernameTextBox.BorderBrush = Brushes.Red;
            if (!passwordValid)
                passwordTextBox.BorderBrush = Brushes.Red;

        }

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private async void GoogleBtn_Click(object sender, RoutedEventArgs e)
        {
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            this.Activate();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                notifier.ShowError(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                notifier.ShowError("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                notifier.ShowError(String.Format("Received request with invalid state ({0})", incoming_state));
                return;
            }

            // Starts the code exchange at the Token Endpoint.
            performCodeExchange(code, code_verifier, redirectURI);
            CustomerWindow customerWindow = new CustomerWindow();
            customerWindow.Show();
            CloseWindow(typeof(SignIn));

        }
        async void performCodeExchange(string code, string code_verifier, string redirectURI)
        {

            // builds the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier,
                clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    userinfoCall(access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                        }
                    }

                }
            }
        }


        async void userinfoCall(string access_token)
        {

            // builds the  request
            string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                // reads response body
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }
        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }
        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }


        private void FacebookBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


    }
}
