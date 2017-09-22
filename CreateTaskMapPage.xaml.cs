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
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;
using Newtonsoft.Json;
using System.Threading.Tasks;
using inz.Resources;

namespace inz
{
    public static class NavigationExtension
    {
        private static object navData = null;

        public static void Navigate(this NavigationService service, string page, object data)
        {
            navData = data;
            service.Navigate(new Uri(page, UriKind.Relative));
        }

        public static object GetNavigationDataObject(this NavigationService service)
        {
            object data = navData;
            navData = null;
            return data;
        }
    }
    public partial class CreateTaskMapPage : PhoneApplicationPage
    {
        public class TaskPin 
        {

            public MapLayer Layer { get; set; }
            public MapOverlay Overlay { get; set; }
            public Pushpin Pp { get; set; }
            public Map TaskMap { get; set; }
            public GeoTask ThisGeoTask{get;set;}

            public TaskPin(Map taskmap, GeoTask geot)
            {
                Layer = new MapLayer();
                Overlay = new MapOverlay();
                Pp = new Pushpin();
                ThisGeoTask = geot;
                TaskMap = taskmap;
            }

            public void GetLocationFromMap(Point mapPoint)
            {
                this.ThisGeoTask.Geo = this.TaskMap.ConvertViewportPointToGeoCoordinate(mapPoint);
            }

            public void SetTaskPin()
            {
                    this.SetPin();
                    this.SetOverlay();
                    this.SetLayer();
            }

            public async Task<bool> GetCurrentLocation()
            {
                try
                {
                    Geolocator myGeolocator = new Geolocator();
                    Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                    Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                    this.ThisGeoTask.Geo = ConvertGeocoordinate(myGeocoordinate);
                    return true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); return false; }
            }

            public Pushpin GetPin() 
            {
                return this.Pp;
            }

            public void SetPin()
            {
                this.Pp.GeoCoordinate = this.ThisGeoTask.Geo;
                this.Pp.Content = this.ThisGeoTask.Tittle;
            }

            public void SetOverlay()
            {
                this.Overlay.Content = Pp;
                this.Overlay.PositionOrigin = new Point(0, 1);
                this.Overlay.GeoCoordinate = this.ThisGeoTask.Geo;
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

            public void SetLayer()
            {
                if (this.Layer.Count != 0) this.Layer.Clear();
                if (this.TaskMap.Layers.Count != 0) this.TaskMap.Layers.Clear();
                this.Layer.Add(this.Overlay);
                this.TaskMap.Layers.Add(this.Layer);
            }
        }

        public TaskPin taskpin { set; get; }
        public GeoTask geoTask { get; set; }

        public CreateTaskMapPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            Map.Tap+=Map_Tap;
        }

        private void CenterMapOnLocation(GeoCoordinate gc)
        {
            this.Map.Center = gc;
            this.Map.ZoomLevel = 13;
        }

        public async Task<bool> GetMyLocation()
        {
            try
            {
                this.taskpin = new TaskPin(this.Map,geoTask);
                Task<bool> getLocation = this.taskpin.GetCurrentLocation();
                await getLocation;
                if (getLocation.Result)
                {
                    this.taskpin.SetTaskPin();
                    CenterMapOnLocation(taskpin.ThisGeoTask.Geo);
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }

        private async void ToggleLocation(object sender, EventArgs e)
        {
            Task<bool> isLocationSet = GetMyLocation();
            await isLocationSet;
            if (!isLocationSet.Result) MessageBox.Show(AppResources.ErrorLocationNotSet); 
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
            NavigationService.GoBack();
            NavigationService.Navigate("/CreateTaskPage.xaml",this.taskpin.Pp);
        }

        private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Point p = new Point();
            p=e.GetPosition(this.Map);
            this.taskpin = new TaskPin(this.Map, geoTask);
            this.taskpin.GetLocationFromMap(p);
            this.taskpin.SetTaskPin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GeoTask myObject = (GeoTask)NavigationService.GetNavigationDataObject();
            if (myObject != null)
            {
                    geoTask = myObject;
                    this.taskpin = new TaskPin(this.Map, geoTask);
                    this.taskpin.SetTaskPin();
                    CenterMapOnLocation(taskpin.ThisGeoTask.Geo);
            }
        }

    }
}