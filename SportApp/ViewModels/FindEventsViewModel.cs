using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;
using FuzzySharp;
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
        public string trao { get; set; } = "shit";

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
            // Command for refreshing the list
            RefreshCommand = new Command(async ()=> await ExecuteRefresh());
            // Command for filtering the list
            //FilterCommandEN = new Command(ApplyFilterEN);
            FilterCommandEN = new Command(async () => await ExecuteRefresh());
            FilterCommandS = new Command(ApplyFilterS);
            FilterCommandC = new Command(ApplyFilterC);
            JoinCommand = new Command<int>(OnJoinEvent);
            Events = new ObservableCollection<Event>();
            IsRefreshing = false;
        }

        private void LoadEventsAsync()
        {
            try
            {
                int i = 1;
                IsRefreshing = true;
                IsRefreshing = false;
                //var eventsList = await proxy.GetEventsAsync();  // Await the task properly
                //Events = new ObservableCollection<Event>(eventsList);
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., network issues)
                Console.WriteLine($"Error loading events: {ex.Message}");
            }
        }
        private async Task ExecuteRefresh()
        {
            // Refresh the list (for example, reload data from a server)
            IsRefreshing = true;
            var e = await proxy.GetEventsAsync();
            try
            {
                Events.Clear();
                foreach (Event s in e)
                    Events.Add(s);
                trao = "2";
                OnPropertyChanged(nameof(trao));
                OnPropertyChanged(nameof(Events));
            }
            catch (Exception ex)
            {
                Console.WriteLine("exeption" + ex.Message);
            }
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
        private void ApplyFilterC()
        {
            if(Users == null || Users.Count == 0)
            {
                Users = new ObservableCollection<Users>(proxyUser.GetEventsAsync().Result);
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
    }
}