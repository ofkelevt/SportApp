using SportApp.Models;
using SportApp.Services;
using SportApp.ViewModels;
using SportApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SportApp.ViewModels
{
    public class LoginViewModel:ViewModel
    {
        private LoginDemoWebAPIProxy service;
        private bool inServerCall;
        public bool InServerCall
        {
            get
            {
                return this.inServerCall;
            }
            set
            {
                this.inServerCall = value;
                OnPropertyChanged(nameof(NotInServerCall));
                OnPropertyChanged(nameof(InServerCall));
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                OnPropertyChanged();
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                OnPropertyChanged();
            }
        }
        public bool NotInServerCall
        {
            get
            {
                return !this.InServerCall;
            }
        }
        public LoginViewModel(LoginDemoWebAPIProxy service)
        {
            InServerCall = false;
            this.service = service;
            this.LoginCommand = new Command(OnLogin);
            this.SignUpCommand = new Command(async () => await OnSignUp());
        }

        public ICommand LoginCommand { get; set; }
        private async void OnLogin()
        {
            //Choose the way you want to blobk the page while indicating a server call
            InServerCall = true;
            //LoginInfo userInfo = new LoginInfo()
            //{
            //    UserName = this.UserName,
            //    Password = this.Password,
            //};
            Users u = await this.service.LoginAsync(UserName,Password);
            
            InServerCall = false;

            //Set the application logged in user to be whatever user returned (null or real user)
            service.LoggedInUser = u;
            if (u == null)
            {

                await Application.Current.MainPage.DisplayAlert("Login", "Login Faild!", "ok");
                var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                shellViewModel.IsLoggedIn = false;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Login", $"Login Succeed! for {u.Username}", "ok");
                var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
                shellViewModel.IsLoggedIn = true;
                await Shell.Current.GoToAsync("//FindEvent");
            }
        }

        public ICommand SignUpCommand { get; set; }
        private async Task OnSignUp()
        {
            var viewModel = new SignUpViewModel();
            var viewEventPage = new SignUpPage(viewModel);
            await Shell.Current.Navigation.PushAsync(viewEventPage);
        }
    }
}
