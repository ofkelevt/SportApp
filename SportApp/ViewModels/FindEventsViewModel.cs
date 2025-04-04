using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;
using FuzzySharp;
using System.Linq;
using Microsoft.VisualBasic;
using SportApp.Views;
using CommunityToolkit.Maui.Converters;

namespace SportApp.ViewModels
{
    public class FindEventsViewModel : ViewModel
    {
        ClientHandler h;
        private EventActionsWebAPIProxy proxy;
        private UserWebAPIProxy proxyUser;
        private ObservableCollection<Event> events;
        private ObservableCollection<Event> originalEvents; // Store original list for filtering
        public ObservableCollection<Event> Events
        {
            get { return events; }
            set
            {
                events = value;
                OnPropertyChanged("Events");
            }
        }
        public ObservableCollection<Users> Users { get; set; }
        public ICommand FilterCommandEN { get; set; }
        public ICommand FilterCommandS { get; set; }
        public ICommand FilterCommandC { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand JoinCommand { get; }
        public ICommand NavigateToUserDetailsCommand { get; }
        public ICommand OnFilterTextChangedENCommand { get; }
        public ICommand OnFilterTextChangedSCommand { get; }

        public ICommand OnFilterTextChangedCCommand { get; }

        private string input1;
        public string Input1
        {
            get { return input1; }
            set
            {
                input1 = value;
                OnPropertyChanged(nameof(Input1));
            }
        }
        private string input2;
        public string Input2
        {
            get { return input2; }
            set
            {
                input2 = value;
                OnPropertyChanged(nameof(Input2));
            }
        }
        private string input3;
        public string Input3
        {
            get { return input3; }
            set
            {
                input3 = value;
                OnPropertyChanged(nameof(Input3));
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public FindEventsViewModel(ClientHandler h)
        {
            this.h = h;
            proxy = new EventActionsWebAPIProxy(h);
            proxyUser = new UserWebAPIProxy(h);
            Events = new ObservableCollection<Event>();
            originalEvents = new ObservableCollection<Event>();
            Users = new ObservableCollection<Users>();

            // Command for refreshing the list
            RefreshCommand = new Command(async () => await ExecuteRefresh());

            // Command for filtering the list
            FilterCommandEN = new Command(async () => await ApplyFilterEN());
            FilterCommandS = new Command(async () => await ApplyFilterS());
            FilterCommandC = new Command(async () => await ApplyFilterC());
            OnFilterTextChangedENCommand = new Command(() => ApplyFilterENRealTime());
            OnFilterTextChangedSCommand = new Command(() => ApplyFilterSRealTime());
            OnFilterTextChangedCCommand = new Command(() => ApplyFilterCRealTime());

            JoinCommand = new Command<Event>(OnJoinEvent);
            NavigateToUserDetailsCommand = new Command<string>(async (u) => await NavigateToUserDetails(u));
            IsRefreshing = true;
        }

        // Real-time filtering methods
        private void ApplyFilterENRealTime()
        {
            if (string.IsNullOrWhiteSpace(Input1) || Input1 == "Event Name filter")
            {
                // If input is empty, restore original list or keep current state
                return;
            }

            var filteredList = originalEvents
                .Where(e => Fuzz.Ratio(Input1, e.EventName) > 50) // Lower threshold for real-time filtering
                .OrderByDescending(e => Fuzz.Ratio(Input1, e.EventName))
                .ToList();

            EventsRestart(filteredList);
        }

        private void ApplyFilterSRealTime()
        {
            if (string.IsNullOrWhiteSpace(Input2) || Input2 == "sport filter")
            {
                // If input is empty, restore original list or keep current state
                return;
            }

            var filteredList = originalEvents
                .Where(e => Fuzz.Ratio(Input2, e.Sport) > 50)
                .OrderByDescending(e => Fuzz.Ratio(Input2, e.Sport))
                .ToList();

            EventsRestart(filteredList);
        }

        private void ApplyFilterCRealTime()
        {
            if (string.IsNullOrWhiteSpace(Input3) || Input3 == "Crator Name filter")
            {
                // If input is empty, restore original list or keep current state
                return;
            }

            if (Users == null || Users.Count == 0)
            {
                // Cannot filter without users
                return;
            }

            var filteredUsers = Users
                .Where(e => Fuzz.Ratio(Input3, e.Username) > 50)
                .OrderByDescending(e => Fuzz.Ratio(Input3, e.Username))
                .ToList();

            var filteredEvents = new ObservableCollection<Event>();
            foreach (var user in filteredUsers)
            {
                foreach (Event evt in originalEvents.Where(s => s.CratorId == user.UserId))
                {
                    filteredEvents.Add(evt);
                }
            }

            EventsRestart(filteredEvents.ToList());
        }

        // Original methods for button clicks
        private async Task ExecuteRefresh()
        {
            // Refresh the list (for example, reload data from a server)
            IsRefreshing = true;
            try
            {
                var u = await proxyUser.GetUsersAsync();
                var fetchedEvents = await proxy.GetEventsAsync();

                EventsRestart(fetchedEvents);

                foreach (var e in Events)
                    e.Crator = u.Where(u => u.UserId == e.CratorId).First();

                // Store original events for filtering
                originalEvents = new ObservableCollection<Event>(Events);

                OnPropertyChanged(nameof(Events));
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception" + ex.Message);
            }

            Input1 = "Event Name filter";
            Input2 = "sport filter";
            Input3 = "Crator Name filter";
            IsRefreshing = false;
        }

        private async Task ApplyFilterEN()
        {
            var input = Input1;
            await ExecuteRefresh();
            Input1 = input;
            var filteredList = Events
                .Where(e => Fuzz.Ratio(input, e.EventName) > 200)
                .OrderByDescending(e => Fuzz.Ratio(input, e.EventName))
                .ToList();
            EventsRestart(filteredList);
        }

        private async Task ApplyFilterS()
        {
            var input = Input2;
            await ExecuteRefresh();
            Input2 = input;
            var filteredList = Events
                .Where(e => Fuzz.Ratio(input, e.Sport) > 50)
                .OrderByDescending(e => Fuzz.Ratio(input, e.Sport))
                .ToList();
            EventsRestart(filteredList);
        }

        private async Task ApplyFilterC()
        {
            var input = Input3;
            await ExecuteRefresh();
            Input3 = input;
            if (Users == null || Users.Count == 0)
            {
                Users = new ObservableCollection<Users>(await proxyUser.GetUsersAsync());
            }
            var filteredList = Users
                .Where(e => Fuzz.Ratio(input, e.Username) > 50)
                .OrderByDescending(e => Fuzz.Ratio(input, e.Username))
                .ToList();

            Users = new ObservableCollection<Users>(filteredList);
            var e = new ObservableCollection<Event>();
            foreach (var user in Users)
            {
                foreach (Event t in Events.Where(s => s.CratorId == user.UserId))
                { e.Add(t); }
            }
            EventsRestart(e.ToList());
        }

        private async void OnJoinEvent(Event selectedEvent)
        {
            if (selectedEvent != null)
            {
                var viewModel = new ViewEventViewModel(selectedEvent, h);
                var viewEventPage = new ViewEvent(viewModel);
                await Shell.Current.Navigation.PushAsync(viewEventPage);
            }
        }

        private void EventsRestart(List<Event> e)
        {
            Events.Clear();
            if (e != null)
                foreach (Event s in e)
                    Events.Add(s);
        }

        private async Task NavigateToUserDetails(string selectedUserName)
        {
            var e = (await proxyUser.GetUsersAsync()).First(u => u.Username == selectedUserName);
            var viewModel = new UserDetailsViewModel(h, e);
            var viewEventPage = new UserDetailsPage(viewModel);
            await Shell.Current.Navigation.PushAsync(viewEventPage);
        }
    }
}