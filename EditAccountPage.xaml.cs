using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using inz.Resources;
using System.Threading.Tasks;

namespace inz
{
    public partial class EditAccountPage : PhoneApplicationPage
    {
        public EditAccountPage()
        {
            InitializeComponent();
        }

        private async void btnDelAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(AppResources.MessageAreYouSure, AppResources.MessageAreYouSureTittle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                User thisUser = new User(Session.Instance.Id);
                Task<bool> userDeleteSuccess = thisUser.Delete();
                await userDeleteSuccess;
                if (userDeleteSuccess.Result)
                {
                    NavigationService.Navigate(new Uri(string.Format("/LoginPage.xaml"), UriKind.Relative));
                    while (this.NavigationService.BackStack.Any())
                    {
                        this.NavigationService.RemoveBackEntry();
                    }

                    MessageBox.Show(AppResources.MessageUserDeleted);
                    Session.Instance.Id = null;
                    Session.Instance.Token = null;
                }
                else MessageBox.Show(AppResources.ErrorUserNotdeleted);
            }

        }

        private async void btnChangePasswd_Click(object sender, RoutedEventArgs e)
        {
            if(IsValid())
            {
                User thisUser = new User(Session.Instance.Id);
                Task <bool> userPasswdChangeSuccess = thisUser.ChangePasswd(editPass.Password, editCPass.Password);
                await userPasswdChangeSuccess;
                if (userPasswdChangeSuccess.Result) { MessageBox.Show(AppResources.MessagePasswdChanged); } else MessageBox.Show(AppResources.MessagePasswdNotChanged);
            }
        }

        private bool IsValid()
        {
            try
            {
                if (editPass.Password.Length < 5)
                {
                    MessageBox.Show(AppResources.ErrorInvalidFieldLengthPass); return false;
                }

                else if (editPass.Password == editCPass.Password)
                {
                    MessageBox.Show(AppResources.ErrorSamePasswd);
                    editPass.Password = "";
                    editCPass.Password = "";
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