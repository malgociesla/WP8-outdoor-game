using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using inz.Resources;

namespace inz
{
    public partial class MapPage : PhoneApplicationPage
    {
        private GeoTask thisGeoTask {get; set;}

        private string Tittle
        {
            get { return (string)GetValue(TittleProperty); }
            set { SetValue(TittleProperty, value); }
        }

        public static readonly DependencyProperty TittleProperty =
           DependencyProperty.Register("Tittle", typeof(string), typeof(MapPage), new PropertyMetadata(null));

        private GeoCoordinate Geo
        {
            get { return (GeoCoordinate)GetValue(GeoProperty); }
            set { SetValue(GeoProperty, value); }
        }

        public static readonly DependencyProperty GeoProperty =
           DependencyProperty.Register("Geo", typeof(GeoCoordinate), typeof(MapPage), new PropertyMetadata(null));

        public MapPage()
        {
            DataContext = this;
            InitializeComponent();
            BuildLocalizedApplicationBar();
            myMap.Tap += Map_Tap;      
        }

        private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Geo= myMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(myMap));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GeoTask myObject = (GeoTask)NavigationService.GetNavigationDataObject();
            if (myObject != null)
            {
                thisGeoTask = myObject;
                Tittle = thisGeoTask.Tittle;
                Geo = thisGeoTask.Geo;
            }
            else thisGeoTask = new GeoTask();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;
            ApplicationBarIconButton localizeBarButton = new ApplicationBarIconButton(new Uri("/Assets/location.png", UriKind.Relative));
            localizeBarButton.Text = "lokalizuj";
            localizeBarButton.Click += ToggleLocation;
            ApplicationBar.Buttons.Add(localizeBarButton);
            ApplicationBarIconButton saveBarButton = new ApplicationBarIconButton(new Uri("/Assets/check.png", UriKind.Relative));
            saveBarButton.Text = "zapisz";
            ApplicationBar.Buttons.Add(saveBarButton);
            saveBarButton.Click += Save;
        }

        private void Save(object sender, EventArgs e)
        {
            thisGeoTask.Latitude = Geo.Latitude;
            thisGeoTask.Longitude = Geo.Longitude;
            ReturnAndEmptyStack();
        }

        void ReturnAndEmptyStack()
        {
            NavigationService.RemoveBackEntry();
            NavigationService.Navigate("/CreateTaskPage.xaml", thisGeoTask);
            NavigationService.RemoveBackEntry();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ReturnAndEmptyStack();
        }

        public async Task<bool> GetCurrentLocation()
        {
            try
            {
                Geolocator myGeolocator = new Geolocator();
                Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                Geo = ConvertGeocoordinate(myGeocoordinate);
                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); return false; }
        }

        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }

        private async void ToggleLocation(object sender, EventArgs e)
        {
            Task<bool> isLocationSet = GetCurrentLocation();
            await isLocationSet;
            if (!isLocationSet.Result) MessageBox.Show(AppResources.ErrorLocationNotSet);
        }
    }
}