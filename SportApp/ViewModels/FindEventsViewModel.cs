using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;
using FuzzySharp;
using System.Linq;
using Microsoft.VisualBasic;
using SportApp.Views;
namespace SportApp.ViewModels
{
    public class FindEventsViewModel : ViewModel
    {
        private EventActionsWebAPIProxy proxy;
        private UserWebAPIProxy proxyUser;
        private ObservableCollection<Event> events;
        public ObservableCollection<Event> Events { get { return events; }
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
        public ICommand fuck { get; }

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

        public FindEventsViewModel()
        {
            proxy = new EventActionsWebAPIProxy();
            proxyUser = new UserWebAPIProxy();
            Events = new ObservableCollection<Event>();
            Users = new ObservableCollection<Users>();
            // Command for refreshing the list
            RefreshCommand = new Command(async ()=> await ExecuteRefresh());
            // Command for filtering the list
            //FilterCommandEN = new Command(ApplyFilterEN);
            FilterCommandEN = new Command(async ()=> await ApplyFilterEN());
            FilterCommandS = new Command(async()=>await ApplyFilterS());
            FilterCommandC = new Command(async ()=> await ApplyFilterC());
            JoinCommand = new Command<Event>(OnJoinEvent);
            fuck = new Command(()=> fucking());
            IsRefreshing = true;
        }
        private async Task ExecuteRefresh()
        {
            // Refresh the list (for example, reload data from a server)
            IsRefreshing = true;
            try
            {
                var u = await proxyUser.GetUsersAsync();
                EventsRestart(await proxy.GetEventsAsync());
                foreach(var e in Events)
                    e.Crator = u.Where(u => u.UserId == e.CratorId).First();
                OnPropertyChanged(nameof(Events));
            }
            catch (Exception ex)
            {
                Console.WriteLine("exeption" + ex.Message);
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
            .Where(e => Fuzz.Ratio(input, e.EventName) > 50) // Filter based on similarity to 'Event' property
            .OrderByDescending(e => Fuzz.Ratio(input, e.EventName)) // Sort by similarity
            .ToList();
            EventsRestart(filteredList);
        }
        private async Task ApplyFilterS()
        {
            var input = Input2;
            await ExecuteRefresh();
            Input2 = input;
            var filteredList = Events
            .Where(e => Fuzz.Ratio(input, e.Sport) > 50) // Filter based on similarity to 'Event' property
            .OrderByDescending(e => Fuzz.Ratio(input, e.Sport)) // Sort by similarity
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
            .Where(e => Fuzz.Ratio(input, e.Username) > 50) // Filter based on similarity to 'Event' property
            .OrderByDescending(e => Fuzz.Ratio(input, e.Username)) // Sort by similarity
            .ToList();

            Users = new ObservableCollection<Users>(filteredList);
            var e = new ObservableCollection<Event>();
            foreach(var user in Users)
            {
                foreach(Event t in Events.Where(s => s.CratorId == user.UserId))
                    { e.Add(t); }
            }
            EventsRestart(e.ToList());
        }
        private async void OnJoinEvent(Event selectedEvent)
        {
            if (selectedEvent != null)
            {
                var viewModel = new ViewEventViewModel(selectedEvent);
                var viewEventPage = new ViewEvent(viewModel);
                await Shell.Current.Navigation.PushAsync(viewEventPage);
            }
        }
        private void EventsRestart(List<Event> e)
        {
            Events.Clear();
            if(e != null)
                foreach (Event s in e)
                    Events.Add(s);
        }
        private void fucking()
        {
            var shellViewModel = (AppShellViewModel)App.Current.MainPage.BindingContext;
            shellViewModel.IsAdmin = true;
            IsRefreshing = true;
        }
    }
}