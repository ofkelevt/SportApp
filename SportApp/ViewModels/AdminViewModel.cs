using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;

namespace SportApp.ViewModels
{
    public class AdminViewModel : ViewModel
    {
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

        public ObservableCollection<Users> Users { get; set; }
        public ICommand RefreshCommand { get; }

        public AdminViewModel()
        {
            _proxyUser = new UserWebAPIProxy();
            _proxyReport = new ReportWebApiProxy();
            _proxyComment = new CommentWebApiProxy();
            _proxyLoginDemo = new LoginDemoWebAPIProxy();
            Users = new ObservableCollection<Users>();
            RefreshCommand = new Command(async () => await RefreshUsersAsync());
            IsRefreshing = true; // Load data initially
        }

        private async Task RefreshUsersAsync()
        {
            IsRefreshing = true;
            try
            {
                await _proxyLoginDemo.CheckAsync();
                if(_proxyLoginDemo.LoggedInUser?.UserId != 2)
                {
                    await Application.Current.MainPage.DisplayAlert("admin page", $"you are not an edmin you can't enter", "ok");
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
                    user.Rating = sum/i;
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
    }
}
