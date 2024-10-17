using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;
using FuzzySharp;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualBasic;
namespace SportApp.ViewModels
{
    public class FindEventsViewModel : ContentView
    {
        private EventActionsWebAPIProxy proxy;
        private EventToUserWebApiProxy proxyConnection;
        private UserWebAPIProxy proxyUser;
        public ObservableCollection<Event> Events { get; set;}
        public ObservableCollection<Users> Users { get; set; }
        public ICommand FilterCommandEN { get; set; }
        public ICommand FilterCommandS { get; set; }
        public ICommand FilterCommandC { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand JoinCommand { get; }

        public string Input1 { get; set; } = "Event Name filter";
        public string Input2 { get; set; } = "sport filter";
        public string Input3 { get; set; } = "Crator Name filter";

        private bool _isRefreshing;
        public bool IsRefreshing
        {

            get { return _isRefreshing; }
            set
            {
                Input1 = "Event Name filter";
                Input2 = "sport filter";
                Input3 = "Crator Name filter";
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public FindEventsViewModel()
        {
            proxy = new EventActionsWebAPIProxy();
            proxyUser = new UserWebAPIProxy();
            proxyConnection = new EventToUserWebApiProxy();
            var e = proxy.GetEventsAsync().Result;
            Events = new ObservableCollection<Event>(e);
            // Command for refreshing the list
            RefreshCommand = new Command(ExecuteRefresh);
            // Command for filtering the list
            FilterCommandEN = new Command(ApplyFilterEN);
            FilterCommandS = new Command(ApplyFilterS);
            FilterCommandC = new Command(ApplyFilterC);
            JoinCommand = new Command<int>(OnJoinEvent);

        }
        private async void ExecuteRefresh()
        {
            // Refresh the list (for example, reload data from a server)
            IsRefreshing = true;
            var e = await proxy.GetEventsAsync();
            Events = new ObservableCollection<Event>(e);
            IsRefreshing = false;
        }

        private void ApplyFilterEN()
        {
            var input = Input1; 
            var filteredList = Events
            .Where(e => Fuzz.Ratio(input, e.EventName) > 50) // Filter based on similarity to 'Event' property
            .OrderByDescending(e => Fuzz.Ratio(input, e.EventName)) // Sort by similarity
            .ToList();

            Events = new ObservableCollection<Event>(filteredList);
        }
        private void ApplyFilterS()
        {

            var input = Input2;
            var filteredList = Events
            .Where(e => Fuzz.Ratio(input, e.Sport) > 50) // Filter based on similarity to 'Event' property
            .OrderByDescending(e => Fuzz.Ratio(input, e.Sport)) // Sort by similarity
            .ToList();

            Events = new ObservableCollection<Event>(filteredList);
        }
        private async void ApplyFilterC()
        {
            if(Users == null || Users.Count == 0)
            {
                Users = new ObservableCollection<Users>(await proxyUser.GetEventsAsync());
                ExecuteRefresh();
            }
            var input = Input3;
            var filteredList = Users
            .Where(e => Fuzz.Ratio(input, e.Username) > 50) // Filter based on similarity to 'Event' property
            .OrderByDescending(e => Fuzz.Ratio(input, e.Username)) // Sort by similarity
            .ToList();

            Users = new ObservableCollection<Users>(filteredList);
            var e = new ObservableCollection<Event>();
            foreach(var user in Users)
            {
                var s = Events.Where(s => s.CratorId == user.UserId);
                foreach(Event t in s)
                    { e.Add(t); }
            }
        }
        private void OnJoinEvent(int eventId)
        {
            // Logic to handle event join (e.g., call API or update UI)
            var selectedEvent = Events.FirstOrDefault(e => e.EventId == eventId);
            if (selectedEvent != null)
            {
                // Add logic to register the user to the event
                Console.WriteLine($"Joined event: {selectedEvent.EventName}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}