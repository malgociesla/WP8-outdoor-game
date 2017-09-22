using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Device.Location;
using Microsoft.Phone.Maps.Toolkit;
using inz.Resources;

namespace inz
{
    public partial class PlayPage : PhoneApplicationPage
    {
        public PlayPage()
        {
            InitializeComponent();
            LoadGeoTasksAsPushPins();
        }

        private async void LoadGeoTasksAsPushPins()
        {
            ObservableCollection<GeoTask> geoTasksColl = new ObservableCollection<GeoTask>();
            geoTasksColl= await GeoTask.LoadPins();
            if (geoTasksColl.Count == 0) MessageBox.Show(AppResources.MessageNoGeoTasks);
            else
            {
                ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(myMap);

                var obj =
                    children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;

                obj.ItemsSource = geoTasksColl;
                myMap.SetView(geoTasksColl[0].Geo, 12.5);
                //myMap.SetView(new GeoCoordinate(geoTasksColl[0].Longitude,geoTasksColl[0].Longitude, 12.1));
            }
        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GeoTask gt = (sender as Pushpin).DataContext as GeoTask;
            NavigationService.Navigate("/ReadTaskPage.xaml", gt);
        }
    }
}