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
using System.Threading.Tasks;


namespace inz
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs ev)
        {
            Redirect();
        }

        protected async void Redirect()
        {
            Task<bool> islogeduser = Session.Instance.IsActive();
            await islogeduser;
            if (islogeduser.Result) NavigationService.Navigate(new Uri(string.Format("/MainPanelPage.xaml"), UriKind.Relative));
        
            else NavigationService.Navigate(new Uri(string.Format("/LoginPage.xaml"), UriKind.Relative));
    }
    }
}