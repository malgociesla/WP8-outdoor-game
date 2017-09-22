using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using inz.Resources;

namespace inz
{
    public partial class UserLog : PhoneApplicationPage
    {
        public UserLog()
        {
            InitializeComponent();
        }

        private void itemPlay_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/PlayPage.xaml"), UriKind.Relative));
        }

        private void itemDefineTasks_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/DefineTasksPage.xaml"), UriKind.Relative));
        }

        private void itemEditAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/EditAccountPage.xaml"), UriKind.Relative));

        }

        private async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Task<bool> logOutSuccess = Session.Instance.Delete();
            await logOutSuccess;
            if (logOutSuccess.Result)
            {
                Session.Instance.Id = null;
                Session.Instance.Token = null;
                NavigationService.Navigate(new Uri(string.Format("/LoginPage.xaml"), UriKind.Relative));
            }
            else MessageBox.Show(AppResources.MessageNotLogedIn);
            while (this.NavigationService.BackStack.Any())
            {
                this.NavigationService.RemoveBackEntry();
            }
        }
    }
}