using SportApp.Models;
using System;
using System.Windows.Input;
using SportApp.Views;
using SportApp.Services;

namespace SportApp.ViewModels
{
    public class CrateEventViewModel : ViewModel
    {
        
        private Event _event;
        private EventActionsWebAPIProxy proxy;
        public DateTime MinimumEndDate { get; } = DateTime.Now;
        public ICommand CreateEventCommand { get; }
        public string EventName { get { return _event.EventName; } set { _event.EventName = value; OnPropertyChanged(nameof(EventName));} }
        public string Sport { get { return _event.Sport; } set { _event.Sport = value; OnPropertyChanged(nameof(Sport)); } }
        public string HomeNum { get { return _event.HomeNum; } set { _event.HomeNum = value; OnPropertyChanged(nameof(HomeNum)); } }
        public string StreetName { get { return _event.StreetName; } set { _event.StreetName = value; OnPropertyChanged(nameof(StreetName)); }}
        public string CityName { get { return _event.CityName; } set { _event.CityName = value; OnPropertyChanged(nameof(CityName)); } }
        public string PictureUrl { get { return _event.PictureUrl; } set { _event.PictureUrl = value; OnPropertyChanged(nameof(PictureUrl)); } }
        public string Description { get { return _event.Description; } set { _event.Description = value; OnPropertyChanged(nameof(Description)); } }
        public DateTime EndsAt { get { return _event.EndsAt; } set { _event.EndsAt = value; OnPropertyChanged(nameof(EndsAt)); } }
        public CrateEventViewModel()
        {
            proxy = new EventActionsWebAPIProxy();
            _event = new Event(); // Initialize the Event object
            CreateEventCommand = new Command(async ()=> await CreateEvent());
        }

        private async Task CreateEvent()
        {
            if (string.IsNullOrWhiteSpace(_event.EventName) ||
                string.IsNullOrWhiteSpace(_event.Sport))
            {
                await App.Current.MainPage.DisplayAlert("Error",
                    "Event Name and Sport are required.", "OK");
                return;
            }

            _event.CreatedAt = DateTime.Now; // Set the current date
            try
            {
                var proxyLogin = new LoginDemoWebAPIProxy();
                var s = await proxyLogin.CheckAsync();
                if (s == "User is not logged in!" || s == "FAILED WITH EXCEPTION!")
                {
                    await App.Current.MainPage.DisplayAlert("Error",
                    "user is not logged in or an error while checking has aqured", "OK");
                    return;
                }
                _event.CratorId = proxyLogin.LoggedInUser.UserId;
            } catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error",
                    $"error: {ex}", "OK");
                return;
            }

            try
            {
                var e = await proxy.PostEventAsync(_event);
                if(!e)
                {
                    await App.Current.MainPage.DisplayAlert("Error",
                    "an error has aquired while saving in database", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error",
                    "an error has aquired while saving in database", "OK");
                return;
            }

            await App.Current.MainPage.DisplayAlert("Success",
                "Event created successfully!", "OK");

            // Navigate back to the events list or another page
            var findeventpage = App.Current.Handler.MauiContext.Services.GetService<FindEvent>();
            await Shell.Current.Navigation.PushAsync(findeventpage);
        }
    }
}
