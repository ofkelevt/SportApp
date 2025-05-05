using SportApp.Models;
using SportApp.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SportApp.ViewModels
{
    public class SignUpViewModel : ViewModel
    {
        private ClientHandler h;
        private Users _user;
        private UserWebAPIProxy proxy;

        public ICommand SignUpCommand { get; }
        public ICommand UploadPictureCommand { get; }

        public SignUpViewModel(ClientHandler h)
        {
            this.h = h;
            _user = new Users(); // Initialize the User object
            proxy = new UserWebAPIProxy(h); // Assume proxy is implemented
            SignUpCommand = new Command(async () => await SignUp()); // Bind the sign-up action
            UploadPictureCommand = new Command(async () => await UploadPicture()); // Bind the upload picture action
        }

        public string Username
        {
            get => _user.Username;
            set { _user.Username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            get => _user.Password;
            set { _user.Password = value; OnPropertyChanged(nameof(Password)); }
        }

        public byte[] PictureUrl
        {
            get => _user.PictureUrl;
            set { _user.PictureUrl = value; OnPropertyChanged(nameof(PictureUrl)); }
        }

        public string FirstName
        {
            get => _user.FirstName;
            set { _user.FirstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        public string LastName
        {
            get => _user.LastName;
            set { _user.LastName = value; OnPropertyChanged(nameof(LastName)); }
        }

        public string PhoneNum
        {
            get => _user.PhoneNum;
            set { _user.PhoneNum = value; OnPropertyChanged(nameof(PhoneNum)); }
        }

        public string? HomeNum
        {
            get => _user.HomeNum;
            set { _user.HomeNum = value; OnPropertyChanged(nameof(HomeNum)); }
        }

        public string? StreetName
        {
            get => _user.StreetName;
            set { _user.StreetName = value; OnPropertyChanged(nameof(StreetName)); }
        }

        public string? CityName
        {
            get => _user.CityName;
            set { _user.CityName = value; OnPropertyChanged(nameof(CityName)); }
        }

        public string? Description
        {
            get => _user.Description;
            set { _user.Description = value; OnPropertyChanged(nameof(Description)); }
        }

        private async Task UploadPicture()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select a Profile Picture"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    PictureUrl = memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., user cancels the file picker)
                Console.WriteLine($"Error picking file: {ex.Message}");
            }
        }

        private async Task SignUp()
        {
            if (string.IsNullOrWhiteSpace(_user.Username) ||
                string.IsNullOrWhiteSpace(_user.Password) ||
                string.IsNullOrWhiteSpace(_user.FirstName) ||
                string.IsNullOrWhiteSpace(_user.LastName))
            {
                await App.Current.MainPage.DisplayAlert("Error",
                    "Username, Password, First Name, and Last Name are required.", "OK");
                return;
            }

            try
            {
                // Assume UserId is set to 0/null on backend to create a new user
                _user.UserId = 0;
                _user.Urank = 1;
                bool success = await proxy.PostUserAsync(_user);
                if (!success)
                {
                        await App.Current.MainPage.DisplayAlert("Error",
                        "Failed to register user.", "OK");
                    return;
                }
                var proxylogin = new LoginDemoWebAPIProxy(h);
                await proxylogin.LoginAsync(Username, Password);

                await App.Current.MainPage.DisplayAlert("Success",
                    "User registered successfully!", "OK");

                // Navigate to login or main page after successful registration
                await Shell.Current.GoToAsync("//FindEvent");

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error",
                    $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
