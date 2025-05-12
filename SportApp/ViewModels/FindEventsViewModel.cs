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
        public ICommand FilterCommand { get; set; }
        public ICommand FilterCommandEN { get; set; }
        public ICommand FilterCommandS { get; set; }
        public ICommand FilterCommandC { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand JoinCommand { get; }
        public ICommand NavigateToUserDetailsCommand { get; }
        public ICommand OnFilterTextChangedENCommand { get; }
        public ICommand OnFilterTextChangedSCommand { get; }

        public ICommand OnFilterTextChangedCCommand { get; }
        public ICommand OnFilterTextChangedCommand { get; }
        public ICommand ReloadCommand { get; }
        private string input;
        public string Input
        {
            get { return input; }
            set
            {
                input = value;
                OnPropertyChanged(nameof(Input));
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
            FilterCommand = new Command(async () => await ApplyFilter());
            OnFilterTextChangedCommand = new Command(()=> ApplyFilterRealTime());

            JoinCommand = new Command<Event>(OnJoinEvent);
            NavigateToUserDetailsCommand = new Command<string>(async (u) => await NavigateToUserDetails(u));
        }

        private async Task ExecuteRefresh()
        {
            // Refresh the list (for example, reload data from a server)
            IsRefreshing = true;
            try
            {
                Users =new ObservableCollection<Users>(await proxyUser.GetUsersAsync());
                var fetchedEvents = await proxy.GetEventsAsync();

                EventsRestart(fetchedEvents);

                foreach (var e in Events)
                {
                    e.Crator = Users.Where(u => u.UserId == e.CratorId).First();
                    e.Crator.pic = (ImageSource)Convert(e.Crator.PictureUrl, typeof(ImageSource), null, null);
                }

                // Store original events for filtering
                originalEvents = new ObservableCollection<Event>(Events);

                OnPropertyChanged(nameof(Events));
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception" + ex.Message);
            }

            Input = "filter";
            IsRefreshing = false;
        }


        // Real-time filtering methods
        private void ApplyFilterRealTime()
        {
            if(string.IsNullOrWhiteSpace(Input) || Input == "filter" || Users == null || Users.Count == 0)
                return;
            var filteredList = originalEvents
                .Where(e => Fuzz.Ratio(Input, e.EventName) > 50) // Lower threshold for real-time filtering
                .OrderByDescending(e => Fuzz.Ratio(Input, e.EventName))
                .ToList();
            var filteredList2 = originalEvents
                .Where(e => Fuzz.Ratio(Input, e.Sport) > 50)
                .OrderByDescending(e => Fuzz.Ratio(Input, e.Sport))
                .ToList();
            foreach(var e in filteredList2.Where(u => !filteredList.Contains(u)))
                filteredList.Add(e);
            var filteredUsers = Users
                .Where(e => Fuzz.Ratio(Input, e.Username) > 50)
                .OrderByDescending(e => Fuzz.Ratio(Input, e.Username))
                .ToList();

            var filteredEvents = new ObservableCollection<Event>();
            foreach (var user in filteredUsers)
            {
                foreach (Event evt in originalEvents.Where(s => s.CratorId == user.UserId))
                {
                    filteredEvents.Add(evt);
                }
            }
            filteredList2 = filteredEvents.ToList();
            foreach (var e in filteredList2.Where(u => !filteredList.Contains(u)))
                filteredList.Add(e);
        }

        // Original methods for button clicks
        private async Task ApplyFilter()
        {
            var input = Input;
            await ExecuteRefresh();
            Input = input;
            var list = ApplyFilterC();
            var list2 = ApplyFilterEN();
            foreach (var item in list2.Where(u => !list.Contains(u)))
                list.Add(item);
            list2 = ApplyFilterS();
            foreach (var item in list2.Where(u => !list.Contains(u)))
                list.Add(item);
            EventsRestart(list.ToList());
        }
        private List<Event> ApplyFilterEN()
        {
            
            var filteredList = Events
                .Where(e => Fuzz.Ratio(input, e.EventName) > 200)
                .OrderByDescending(e => Fuzz.Ratio(Input, e.EventName))
                .ToList();
            return filteredList;
        }

        private List<Event> ApplyFilterS()
        {
            var filteredList = Events
                .Where(e => Fuzz.Ratio(Input, e.Sport) > 50)
                .OrderByDescending(e => Fuzz.Ratio(Input, e.Sport))
                .ToList();
            return filteredList;
        }

        private List<Event> ApplyFilterC()
        {
            var filteredList = Users
                .Where(e => Fuzz.Ratio(Input, e.Username) > 50)
                .OrderByDescending(e => Fuzz.Ratio(Input, e.Username))
                .ToList();

            Users = new ObservableCollection<Users>(filteredList);
            var e = new ObservableCollection<Event>();
            foreach (var user in Users)
            {
                foreach (Event t in Events.Where(s => s.CratorId == user.UserId))
                { e.Add(t); }
            }
            return e.ToList();
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[] byteArray && byteArray.Length > 0)
            {
                return ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
            return null; // Return a default image or null if no data is available
        }
    }
}