using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Converters;
using SportApp.Services;
using SportApp.Views;

namespace SportApp.ViewModels
{
    public class LogoutViewModel : ViewModel
    {
        private LoginDemoWebAPIProxy _proxy;
        public ICommand LogoutCommand { get; }
        private bool isRefreshing;
        public bool IsRefreshing { get { return isRefreshing; } set { isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }
        public LogoutViewModel() 
        {
            LogoutCommand = new Command(async () => await OnLogout());
            _proxy = new LoginDemoWebAPIProxy();
            IsRefreshing = true;
        }
        private async Task OnLogout()
        {
            try
            {
                var e = await _proxy.Logout();
                if (!e)
                {
                    await Application.Current.MainPage.DisplayAlert("logout", $"error", "ok");
                    return;
                }
                await Application.Current.MainPage.DisplayAlert("logout", $"success", "ok");
                var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                shellViewModel.IsLoggedIn = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("logout", $"error: {ex}", "ok");
            }
            finally
            {
                await Shell.Current.GoToAsync("//FindEvent");

            }
        }
    }
}
