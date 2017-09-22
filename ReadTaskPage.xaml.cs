using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace inz
{
    public partial class ReadTaskPage : PhoneApplicationPage
    {
        public ReadTaskPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GeoTask myObject = (GeoTask)NavigationService.GetNavigationDataObject();
            if (myObject != null)
            {
                this.DataContext = myObject;
            }
        }
    }
}