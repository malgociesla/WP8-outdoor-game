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
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Input;

namespace inz
{
    public partial class CreateTaskPage : PhoneApplicationPage
    {
        private GeoTask thisGeoTask {get; set;}

        public CreateTaskPage()
        {
            InitializeComponent();
            this.taskName.KeyUp += txt_KeyUp;
            this.taskDesc.KeyUp += txt_KeyUp;
            this.taskLat.KeyUp += txt_KeyUp;
            this.taskLon.KeyUp += txt_KeyUp;
        }

        void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }
        private bool IsValid()
        {
            try
            {
                if (taskName.Text.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLength5); return false;
                }
                else if (taskDesc.Text.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLength5); return false;
                }
                else if (this.taskLat.Text.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLength5); return false;
                }
                else if (this.taskLon.Text.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLength5); return false;
                }
                else if (Convert.ToDouble(this.taskLat.Text) > 90 || Convert.ToDouble(this.taskLat.Text) < -90 || Convert.ToDouble(this.taskLon.Text) > 90 || Convert.ToDouble(this.taskLon.Text) < -90)
                {
                    MessageBox.Show(AppResources.ErrorInvalidLocalization); return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); return false;
            }
        }

        private async void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                if (thisGeoTask.Id != null)
                {
                    Task<bool> isAlready = thisGeoTask.FindTask();
                    await isAlready;
                    if (isAlready.Result)
                    {
                        Task<bool> isUpdated = thisGeoTask.UpdateTask();
                        await isUpdated;
                        if (isUpdated.Result) MessageBox.Show(AppResources.MessageTaskUpdated);
                    }
                }
                else
                {
                    Task<bool> isCompleted = thisGeoTask.AddTask();
                    await isCompleted;
                    if (isCompleted.Result)
                    {
                        MessageBox.Show(AppResources.MessageTaskAdded);
                    }
                }
            }
        }

        private void btnAddPin_Click(object sender, RoutedEventArgs e)
        {
            if(IsValid())NavigationService.Navigate("/MapPage.xaml", thisGeoTask);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
                GeoTask myObject = (GeoTask)NavigationService.GetNavigationDataObject();
                if (myObject != null)
                {
                    thisGeoTask = myObject;
                }
                else thisGeoTask = new GeoTask();
                this.DataContext = thisGeoTask;
        }
    }
}