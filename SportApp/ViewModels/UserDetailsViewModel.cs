using SportApp.Models;
using SportApp.Services;
using SportApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
namespace SportApp.ViewModels
{
    public class UserDetailsViewModel : ViewModel
    {
        private UserWebAPIProxy proxyUser;
        private LoginDemoWebAPIProxy proxyLogin;
        private ReportWebApiProxy proxyReport;
        private CommentWebApiProxy proxyComment;
        private EventActionsWebAPIProxy proxyEvent;
        private ChatCommentWebAPIProxy proxyChatComment;
        private EventToUserWebApiProxy proxyEventToUser;
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
        public int Rating { get { return rating; } set { rating = value; OnPropertyChanged(nameof(Rating)); UpdateStarImages(); } }
        public string Star1Image => rating >= 1 ? "filled_star.png" : "empty_star.png";
        public string Star2Image => rating >= 2 ? "filled_star.png" : "empty_star.png";
        public string Star3Image => rating >= 3 ? "filled_star.png" : "empty_star.png";
        public string Star4Image => rating >= 4 ? "filled_star.png" : "empty_star.png";
        public string Star5Image => rating >= 5 ? "filled_star.png" : "empty_star.png";
        private bool isAdmin;
        public bool IsAdmin { 
            get { return isAdmin; } 
            set { isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin)); } }
        private int span;
        public int Span { get { return span; } set { span = value; OnPropertyChanged(nameof(Span)); } }
        public Command RefreshCommand { get; }
        public Command CommentCommand { get; }
        public Command ReportCommand { get; }
        public Command BanCommand { get; }
        public Command DelteCommentCommand { get; }
        public Command OnSave { get; }
        public ICommand SetRatingCommand => new Command<int>((u)=> Rating = u);
        public ICommand NavigateToUserDetailsCommand => new Command<Users>(async (u) => await NavigateToUserDetails(u));

        public UserDetailsViewModel(Users user = null)
        {
            User = user;
            proxyLogin = new LoginDemoWebAPIProxy();
            proxyUser = new UserWebAPIProxy();
            proxyComment = new CommentWebApiProxy();
            proxyReport = new ReportWebApiProxy();
            proxyEvent = new EventActionsWebAPIProxy();
            proxyChatComment = new ChatCommentWebAPIProxy();
            proxyEventToUser = new EventToUserWebApiProxy();
            
            Comments = new ObservableCollection<Comment>();
            Reports = new ObservableCollection<Report>();

            RefreshCommand = new Command(async () => await OnRefresh());
            CommentCommand = new Command<string>(async (commentText) =>
                await OnComment(commentText, Rating));
            ReportCommand = new Command<string>(async (reportText) =>
                await OnReport(reportText));
            BanCommand = new Command(async () => await OnBan());
            OnSave = new Command(async () => await Save());
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
                    IsLoggedIn = true;
                }
                else
                {
                    if(proxyLogin.LoggedInUser != null)
                    {
                        IsLoggedUser = proxyLogin.LoggedInUser.UserId == User.UserId;
                        IsLoggedIn = true;
                    }
                    else
                    {
                        IsLoggedIn = false;
                        IsLoggedUser = false;
                    }

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
                var events = (await proxyEvent.GetEventsAsync()).Where(u => u.CratorId == User.UserId);
                var t = new List<Event>(events);
                foreach(var ee in t)
                {
                    var s = (await proxyEventToUser.GetEventsAsync()).Where(u => u.EventId == ee.EventId);
                    foreach (var s2 in s)
                        await proxyEventToUser.DeleteEventAsync(s2.TableId);
                    await proxyEvent.DeleteEventAsync(ee.EventId);
                }
                var comment = await proxyComment.GetUserCommentsAsync(User.UserId);
                var tempcomment = new List<Comment>(comment);
                foreach(var ee in  tempcomment)
                    await proxyComment.DeleteCommentAsync(ee.CommentId);
                var chatcomment = (await proxyChatComment.GetChatCommentsAsync()).Where(u => u.CommenterId == User.UserId); ;
                var tempchatcomment = new List<ChatComment>(chatcomment);
                foreach (var ee in tempchatcomment)
                    await proxyChatComment.DeleteChatCommentAsync(ee.CommentId);
                var report = (await proxyReport.GetUserReportsAsync(User.UserId));
                var tempreport = new List<Report>(report);
                foreach (var ee in tempreport)
                    await proxyReport.DeleteReportAsync(ee.ReportId);
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
        private async Task Save()
        {
            try
            {
                var e = await proxyUser.PutUserAsync(User);
                if (!e)
                {
                    await Application.Current.MainPage.DisplayAlert("Save changes", $"error while saving changes", "ok");
                }
                await Application.Current.MainPage.DisplayAlert("Save changes", $"seccuss", "ok");
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Save chnages", $"error while saving changes :{ex}", "ok");
            }
            finally { IsRefreshing = true; }

        }
        private void UpdateStarImages()
        {
            OnPropertyChanged(nameof(Star1Image));
            OnPropertyChanged(nameof(Star2Image));
            OnPropertyChanged(nameof(Star3Image));
            OnPropertyChanged(nameof(Star4Image));
            OnPropertyChanged(nameof(Star5Image));
        }
        private async Task NavigateToUserDetails(Users selectedUser)
        {
            if (selectedUser != null)
            {
                var viewModel = new UserDetailsViewModel(selectedUser);
                var viewEventPage = new UserDetailsPage(viewModel);
                await Shell.Current.Navigation.PushAsync(viewEventPage);
            }
        }
    }
}
