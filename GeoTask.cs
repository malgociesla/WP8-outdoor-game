using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace inz
{
    public class GeoTask : DependencyObject
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tittle")]
        public string Tittle { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }

        //[JsonIgnore]
        // public GeoCoordinate Geo {
        //    get { return (GeoCoordinate) GetValue(GeoProperty); }
        //    set { SetValue(GeoProperty, value); SetGeo(); }
        //}

        //public static readonly DependencyProperty GeoProperty =
        //   DependencyProperty.Register("Geo", typeof(GeoCoordinate), typeof(GeoTask), new PropertyMetadata(null));
     

        [JsonIgnore]
        public GeoCoordinate Geo { get { return GetGeo(); } set { SetGeo(); } }

        public GeoTask()
        {
            Owner = Session.Instance.Id;
        }

        public GeoTask(string name, string desc)
        {
            Tittle = name;
            Description = desc;
            Owner = Session.Instance.Id;
        }

        public GeoTask(string name, string desc, double lat, double lon)
        {
            Tittle = name;
            Description = desc;
            Latitude = lat;
            Longitude=lon;
            Owner = Session.Instance.Id;
        }

        public static async Task<ObservableCollection<GeoTask>> LoadPins()
        {
            try
            {
                 IMobileServiceTable<GeoTask> taskTable = App.MobileService.GetTable<GeoTask>();
                return new ObservableCollection<GeoTask>(await taskTable.ToEnumerableAsync());
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }
        }

        public static async Task<ObservableCollection<GeoTask>> UserTasks()
        {
            try
            {
                IMobileServiceTable<GeoTask> taskTable = App.MobileService.GetTable<GeoTask>();
                return new ObservableCollection<GeoTask>(await taskTable.Where(x => x.Owner == Session.Instance.Id).ToEnumerableAsync());
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }
        }

        public void SetGeo()
        {
            this.Latitude = this.Geo.Latitude;
            this.Longitude = this.Geo.Longitude;
        }

        public GeoCoordinate GetGeo()
        {
            return new GeoCoordinate(this.Latitude, this.Longitude);
        }

        public async Task<bool> AddTask()
        {
            try
            {
                IMobileServiceTable<GeoTask> taskTable = App.MobileService.GetTable<GeoTask>();
                Task isInserted = taskTable.InsertAsync(this);
                await isInserted;
                if (isInserted.IsCompleted) return true;
                else return false;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }

        public async Task<bool> UpdateTask() 
        {
            try
            {
                IMobileServiceTable<GeoTask> taskTable = App.MobileService.GetTable<GeoTask>();
                Task isUpdated = taskTable.UpdateAsync(this);
                await isUpdated;
                if (isUpdated.IsCompleted) return true;
                else return false;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }

        public async Task<bool> FindTask() 
        {
            try
            {
                IMobileServiceTable<GeoTask> taskTable = App.MobileService.GetTable<GeoTask>();
                Task isAlready = taskTable.LookupAsync(this.Id);
                await isAlready;
                if (isAlready.IsCompleted) return true;
                else return false;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }

        public async Task<bool> DeleteTask()
        {
            try
            {
                IMobileServiceTable<GeoTask> taskTable = App.MobileService.GetTable<GeoTask>();
                Task isDeleted = taskTable.DeleteAsync(this);
                await isDeleted;
                if (isDeleted.IsCompleted) return true;
                else return false;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
    }

}
