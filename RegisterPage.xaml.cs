using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using inz.Resources;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Threading.Tasks;

namespace inz
{
    public partial class Register : PhoneApplicationPage
    {
        private IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();

        public Register()
        {
            InitializeComponent();
            this.regName.KeyUp += txt_KeyUp;
            this.regPass.KeyUp += txt_KeyUp;
            this.regCPass.KeyUp += txt_KeyUp;
            this.regEmail.KeyUp += txt_KeyUp;
        }

        void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        { 
            if (IsValid())
            {
                btnRegister.IsEnabled = false;
                User user = new User(regName.Text, regPass.Password, regEmail.Text);
                Task<bool> isCompleted = user.AddUser();
                await isCompleted;
                if (isCompleted.Result) { MessageBox.Show(AppResources.MessageRegistered);  NavigationService.Navigate(new Uri(string.Format("/LoginPage.xaml"), UriKind.Relative)); }
                btnRegister.IsEnabled = true;
            }
        }

        private bool IsValid()
        {
            try
            {
                if (regName.Text.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLengthName); return false;
                }
                else if (regPass.Password.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLengthPass); return false;
                }
                else if (regEmail.Text.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLengthEmail); return false;
                }
                else if (regPass.Password != regCPass.Password)
                {
                    MessageBox.Show(AppResources.ErrorDifferentPasswords);
                    regPass.Password = "";
                    regCPass.Password = "";
                    return false;
                }
                else if (!User.IsEmailValid(regEmail.Text))
                {
                    MessageBox.Show(AppResources.ErrorInvalidEmail);
                    return false;
                }
                else return true;
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); return false;
            }
        }
    }
}