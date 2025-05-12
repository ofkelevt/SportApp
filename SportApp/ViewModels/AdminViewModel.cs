using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;
using SportApp.Views;

namespace SportApp.ViewModels
{
    public class AdminViewModel : ViewModel
    {
        private ClientHandler h;
        private readonly UserWebAPIProxy _proxyUser;
        private readonly ReportWebApiProxy _proxyReport;
        private readonly CommentWebApiProxy _proxyComment;
        private readonly LoginDemoWebAPIProxy _proxyLoginDemo;
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Users> users;
        public ObservableCollection<Users> Users { get => users; set { users = value; OnPropertyChanged(); } }
        public ICommand RefreshCommand { get; }
        public ICommand NavigateToUserDetailsCommand => new Command<Users>(async (u) => await NavigateToUserDetails(u));
        public AdminViewModel(ClientHandler h)
        {
            this.h = h;
            _proxyUser = new UserWebAPIProxy(h);
            _proxyReport = new ReportWebApiProxy(h);
            _proxyComment = new CommentWebApiProxy(h);
            _proxyLoginDemo = new LoginDemoWebAPIProxy(h);
            Users = new ObservableCollection<Users>();
            RefreshCommand = new Command(async () => await RefreshUsersAsync());
        }

        private async Task RefreshUsersAsync()
        {
            IsRefreshing = true;
            try
            {
                await _proxyLoginDemo.CheckAsync();
                if(_proxyLoginDemo.LoggedInUser?.Urank != 2)
                {
                    await Application.Current.MainPage.DisplayAlert("admin page", $"you are not an admin you can't enter", "ok");
                    await Shell.Current.GoToAsync("//FindEvent");
                }
                var users = await _proxyUser.GetUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    var reports = await _proxyReport.GetUserReportsAsync(user.UserId);
                    var comments = await _proxyComment.GetUserCommentsAsync(user.UserId);
                    user.ReportCount = reports.Count;
                    int sum = 0;
                    int i = 0;
                    foreach (var c in comments)
                    {
                        sum += c.Rating;
                        i++;
                    }
                    if (i != 0)
                        user.Rating = sum / i;
                    else user.Rating = -1;
                    if(Users.Where(u => u.UserId == user.UserId).FirstOrDefault()==null)
                        Users.Add(user);
                    
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Failed to load users: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        private async Task NavigateToUserDetails(Users selectedUser)
        {
            if (selectedUser != null)
            {
                var viewModel = new UserDetailsViewModel(h,selectedUser);
                var viewEventPage = new UserDetailsPage(viewModel);
                await Shell.Current.Navigation.PushAsync(viewEventPage);
            }
        }
    }
}
