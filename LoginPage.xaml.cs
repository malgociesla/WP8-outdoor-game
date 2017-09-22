using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace inz
{
    public partial class Login : PhoneApplicationPage
    {                
        public Login()
        {
            InitializeComponent();
        
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
            this.txtName.KeyUp += txt_KeyUp;
            this.txtPass.KeyUp += txt_KeyUp;
        }

        public void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        private async void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            Task<bool> isloguser = User.LogIn(txtName.Text, txtPass.Password);
            btnLogIn.IsEnabled = false;
            btnRegister.IsEnabled = false;
            await isloguser;
            if (isloguser.Result) NavigationService.Navigate(new Uri(string.Format("/MainPanelPage.xaml"), UriKind.Relative));
            btnLogIn.IsEnabled = true;
            btnRegister.IsEnabled = true;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/RegisterPage.xaml"), UriKind.Relative));
        }
            
     }
}