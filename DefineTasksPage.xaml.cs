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
using System.Collections.ObjectModel;
using inz.Resources;
using System.Threading.Tasks;

namespace inz
{
    public partial class DefineTasksPage : PhoneApplicationPage
    {
        public DefineTasksPage()
        {
            InitializeComponent();
        }

        private async void RefreshTasks() 
        {
            try 
            { 
                 ObservableCollection<GeoTask> geoTasksColl = new ObservableCollection<GeoTask>();
                 geoTasksColl= await GeoTask.UserTasks();
                 ListItems.ItemsSource = geoTasksColl;
                 if (geoTasksColl.Count == 0) MessageBox.Show(AppResources.MessageNoGeoTasks);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        } 

        protected override void OnNavigatedTo(NavigationEventArgs ev)
        {
            RefreshTasks();
        }

        private void btnNewTask_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/CreateTaskPage.xaml"), UriKind.Relative));
        }

        private async void ImageDelete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GeoTask gt = (sender as Image).DataContext as GeoTask;
            if (MessageBox.Show(AppResources.MessageDeleteTask, AppResources.MessageAreYouSureTittle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Task<bool> delTaskSuccess = gt.DeleteTask();
                await delTaskSuccess;
                if (delTaskSuccess.Result) { MessageBox.Show(AppResources.MessageTaskDeleted); }
                RefreshTasks();
            }
        }

        private void ImageEdit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GeoTask gt = (sender as Image).DataContext as GeoTask;
            NavigationService.Navigate("/CreateTaskPage.xaml", gt);
        }
    }
}