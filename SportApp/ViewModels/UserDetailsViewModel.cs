using SportApp.Models;
using SportApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace SportApp.ViewModels
{
    public class UserDetailsViewModel : ViewModel
    {
        private UserWebAPIProxy proxyUser;
        private LoginDemoWebAPIProxy proxyLogin;
        private ReportWebApiProxy proxyReport;
        private CommentWebApiProxy proxyComment;
        private Users _user;
        private ObservableCollection<Comment> comments;
        public ObservableCollection<Comment> Comments { get { return comments; } set { comments = value; OnPropertyChanged(nameof(Comments)); } }
        private ObservableCollection<Report> reports;
        public ObservableCollection<Report> Reports { get { return reports; } set { reports = value; OnPropertyChanged(nameof(Reports)); } }
        private bool _isLoggedUser;

        public Users User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedUser
        {
            get => _isLoggedUser;
            set
            {
                _isLoggedUser = value;
                OnPropertyChanged();
            }
        }
        private bool isLoggedIn;
        public bool IsLoggedIn { get { return isLoggedIn;} set { isLoggedIn = value; OnPropertyChanged(nameof(IsLoggedIn)); } }
        private bool isRefreshing;
        public bool IsRefreshing { get { return isRefreshing; } set { isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }
        private int rating;
        public int Rating { get { return rating; } set { rating = value; OnPropertyChanged(nameof(Rating)); } }
        private bool isAdmin;
        public bool IsAdmin { get { return isAdmin; } set { isAdmin = value;OnPropertyChanged(nameof(IsAdmin)); } }
        private int span;
        public int Span { get { return span; } set { span = value; OnPropertyChanged(nameof(Span)); } }
        public Command RefreshCommand { get; }
        public Command CommentCommand { get; }
        public Command ReportCommand { get; }
        public Command BanCommand { get; }
        public Command DelteCommentCommand { get; }

        public UserDetailsViewModel(Users user = null)
        {
            User = user;
            proxyLogin = new LoginDemoWebAPIProxy();
            proxyUser = new UserWebAPIProxy();
            proxyComment = new CommentWebApiProxy();
            proxyReport = new ReportWebApiProxy();
            Comments = new ObservableCollection<Comment>();
            Reports = new ObservableCollection<Report>();

            RefreshCommand = new Command(async () => await OnRefresh());
            CommentCommand = new Command<string>(async (commentText) =>
                await OnComment(commentText, Rating));
            ReportCommand = new Command<string>(async (reportText) =>
                await OnReport(reportText));
            BanCommand = new Command(async () => await OnBan());
            DelteCommentCommand = new Command<Comment>(async (u) => await OnDelteComment(u));
            IsRefreshing = true;
        }
        private async Task OnRefresh()
        {
            IsRefreshing = true;
            try
            {
                await proxyLogin.CheckAsync();
                if (User == null)
                {
                    User = proxyLogin.LoggedInUser;
                    IsLoggedUser = true;
                }
                else
                {
                    IsLoggedUser = proxyLogin.LoggedInUser.UserId == User.UserId;
                }
                IsAdmin = false;
                if (proxyLogin.LoggedInUser.Urank == 2)
                {
                    IsAdmin = true;
                }
                Span = 1;
                if (!IsAdmin)
                    Span++;
                var users = await proxyUser.GetUsersAsync();
                var s = await proxyComment.GetUserCommentsAsync(User.UserId);
                Comments.Clear();
                int i = 0;
                int sum = 0;
                foreach (var comment in s)
                {
                    comment.Commenter = users.First(u => u.UserId == comment.CommenterId);
                    comment.IsCommenter = comment.CommenterId == proxyLogin.LoggedInUser.UserId || proxyLogin.LoggedInUser.Urank == 2;
                    Comments.Add(comment);
                    sum += comment.Rating;
                    i++;
                }
                OnPropertyChanged(nameof(Comments));
                var t = await proxyReport.GetUserReportsAsync(User.UserId);
                Reports.Clear();
                foreach(var report in t)
                {
                    report.Reporter = users.First(u => u.UserId == report.ReporterId);
                    Reports.Add(report);
                }
                OnPropertyChanged(nameof(Reports));
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("error",
                    $"error:{ex}", "OK");
            }
            IsRefreshing = false;
        }
        public async Task OnComment(string comment, int rating)
        {
            if (!isLoggedIn)
            {
                await App.Current.MainPage.DisplayAlert("comment error",
                   $"you have to be logged in to comment", "OK");
                return;
            }
            if(IsLoggedUser)
            {
                await App.Current.MainPage.DisplayAlert("comment error",
                   $"you can't comment on  your self", "OK");
                return;
            }
            try
            {
                var e = await proxyComment.PostUserAsync(new Comment() { CommentedOnId = User.UserId, CommenterId = proxyLogin.LoggedInUser.UserId, CommentText = comment, Rating = rating, CreatedAt = DateTime.Now });
                if (!e)
                {
                    await App.Current.MainPage.DisplayAlert("comment error",
                   $"error sending comment to server", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("comment error",
                  $"error sending comment to server {ex}", "OK");
                return;
            } finally
            {
                IsRefreshing = true;
            }
        }
        private async Task OnReport(string comment)
        {
            if (!isLoggedIn)
            {
                await App.Current.MainPage.DisplayAlert("Report error",
                   $"you have to be logged in to report", "OK");
                return;
            }
            if (IsLoggedUser)
            {
                await App.Current.MainPage.DisplayAlert("Report error",
                   $"you can't report on  your self", "OK");
                return;
            }
            try
            {
                var e = await proxyReport.PostUserAsync(new Report() { CommentText = comment, CreatedAt = DateTime.Now, ReporterId = proxyLogin.LoggedInUser.UserId, TargetId = User.UserId});
                if (!e)
                {
                    await App.Current.MainPage.DisplayAlert("Report error",
                   $"error sending report to server", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Report error",
                  $"error sending report to server {ex}", "OK");
                return;
            }
            finally
            {
                IsRefreshing = true;
            }
        }
        private async Task OnBan()
        {
            if(User.Urank == 2)
            {
                await App.Current.MainPage.DisplayAlert("ban error",
                   $"you can't ban an admin", "OK");
                return;
            }
            try
            {
                bool e = await proxyUser.DeleteUserAsync(User.UserId);
                if(!e)
                {
                    await App.Current.MainPage.DisplayAlert("ban error",
                   $"error while delting user", "OK");
                    await Shell.Current.GoToAsync("//FindEvent");
                    return;
                }
                await App.Current.MainPage.DisplayAlert("ban",
                   $"user delted successfuly", "OK");
                await Shell.Current.GoToAsync("//FindEvent");
            }catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("ban error",
                   $"error while delting user: {ex}", "OK");
                await Shell.Current.GoToAsync("//FindEvent");
                return;
            }



        }
        private async Task OnDelteComment(Comment comment)
        {
            try
            {
                bool e = await proxyComment.DeleteCommentAsync(comment.CommentId);
                if(!e)
                {
                    await Application.Current.MainPage.DisplayAlert("Delete Comment", $"error while delting comment", "ok");
                    return;
                }
                IsRefreshing = true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Delete Comment", $"error: {ex}", "ok");
            }

        }
    }
}
