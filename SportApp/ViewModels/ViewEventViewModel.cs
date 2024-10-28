using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SportApp.Models;
using SportApp.Services;
using SportApp.Views;

namespace SportApp.ViewModels
{
    public class ViewEventViewModel : ViewModel
    {
        private Event selcetedEvent;
        public Event SelectedEvent { get { return selcetedEvent; }
            set 
            {
                selcetedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));
            }
        }
        private EventActionsWebAPIProxy proxyEvent;
        private EventToUserWebApiProxy proxyEventToUser;
        private UserWebAPIProxy proxyUser;
        private ChatCommentWebAPIProxy proxyChatComment; 
        private LoginDemoWebAPIProxy proxyloginDemoWebAPI;

        private ObservableCollection<Users> users;
        public ObservableCollection<Users> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged(nameof(Users));
            }
        }
        private ObservableCollection<Users> waitListedUsers;
        public ObservableCollection<Users> WaitListedUsers
        {
            get { return waitListedUsers; }
            set
            {
                waitListedUsers = value;
                OnPropertyChanged(nameof(WaitListedUsers));
            }
        }
        public ObservableCollection<UserToEvent> UserToEvents{get;set;}
        private ObservableCollection<ChatComment> chatComments;
        public ObservableCollection<ChatComment> ChatComments { get { return chatComments; } set { chatComments = value; OnPropertyChanged(nameof(ChatComments));} }
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public ICommand JoinCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand KickCommand { get; }
        public ICommand WaitlistCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand AcceptCommand { get; }
        public ICommand CommentCommand { get; }
        public ICommand DelteCommantCommand { get; }
        public ICommand LeaveCommand { get; }

        private bool isCreator;
        public bool IsCreator { get { return isCreator; } set {isCreator = value; OnPropertyChanged(nameof(IsCreator)); } }
        private bool isJoin;
        public bool IsJoin { get { return isJoin; } set { isJoin = value; OnPropertyChanged(nameof(IsJoin)); } }
        private bool isCratorOrJoin;
        public bool IsCratorOrJoin { get { return isCratorOrJoin; } set {isCratorOrJoin = value; OnPropertyChanged(nameof(IsCratorOrJoin));
            } }
        private string commentText;
        public string CommentText { get { return commentText; } set { commentText = value; OnPropertyChanged(nameof(CommentText)); } } 

        public ViewEventViewModel(Event selectedEvent)
        {
            
            SelectedEvent = selectedEvent;
            proxyEvent = new EventActionsWebAPIProxy();
            proxyEventToUser = new EventToUserWebApiProxy();
            proxyUser = new UserWebAPIProxy();
            proxyloginDemoWebAPI = new LoginDemoWebAPIProxy();
            proxyChatComment = new ChatCommentWebAPIProxy();
            // Example: Fill Users and UserToEvents collections (assuming they're already populated)
            Users = new ObservableCollection<Users>();
            chatComments = new ObservableCollection<ChatComment>();
            waitListedUsers = new ObservableCollection<Users>();
            UserToEvents = new ObservableCollection<UserToEvent>();
            RefreshCommand = new Command(async ()=> await Refresh());
            JoinCommand = new Command(async ()=> await OnJoin());
            SaveCommand = new Command(async () => await Save());
            KickCommand = new Command(async () => await OnKickUsers());
            WaitlistCommand = new Command(async () => await OnWaitlistUsers());
            RejectCommand = new Command(async () => await OnRejectUsers());
            AcceptCommand = new Command(async () => await OnAcceptUsers());
            CommentCommand = new Command(async ()=> await Comment());
            DelteCommantCommand = new Command<ChatComment>(async (u) => await DelteComment(u));
            LeaveCommand = new Command(async ()=> await OnLeave());
            IsRefreshing = true;
        }
        private async Task Refresh()
        {
            IsRefreshing = true;
            try
            {
                CommentText = "write comment text";
                SelectedEvent = await proxyEvent.GetEventAsync(SelectedEvent.EventId);
                SelectedEvent.Crator = await proxyUser.GetUserAsync(SelectedEvent.CratorId);
                OnPropertyChanged(nameof(SelectedEvent));
                var t = await proxyloginDemoWebAPI.CheckAsync();
                IsCreator = (proxyloginDemoWebAPI.LoggedInUser != null && SelectedEvent.CratorId == proxyloginDemoWebAPI.LoggedInUser.UserId);
                ChatComments.Clear();
                var s = await proxyChatComment.GetEventChatCommentsAsync(SelectedEvent.EventId);
                if (s != null)
                    foreach (var comment in s)
                    {
                        comment.Commenter = await proxyUser.GetUserAsync(comment.CommenterId);
                        comment.IsCommenter = IsCreator || (proxyloginDemoWebAPI.LoggedInUser != null && comment.CommenterId == proxyloginDemoWebAPI.LoggedInUser.UserId);
                        ChatComments.Add(comment);
                    }
                OnPropertyChanged(nameof(ChatComments));
                UserToEvents = new ObservableCollection<UserToEvent>((await proxyEventToUser.GetEventsAsync()).Where(e => e.EventId == SelectedEvent.EventId));
                IsJoin = (proxyloginDemoWebAPI.LoggedInUser != null && UserToEvents.Where(u => u.UserId == proxyloginDemoWebAPI.LoggedInUser.UserId).Count() > 0);
                IsCratorOrJoin = IsJoin || IsCreator;
                //attend
                Users.Clear();
                //waiting
                WaitListedUsers.Clear();
                foreach (var e in UserToEvents)
                {
                    if (e.RealtionshipType == "attend")
                        Users.Add(await proxyUser.GetUserAsync(e.UserId));
                    else
                        WaitListedUsers.Add(await proxyUser.GetUserAsync(e.UserId));
                }
                foreach (var e in Users)
                    e.IsCheck = false;
                foreach (var e in waitListedUsers)
                    e.IsCheck = false;
                OnPropertyChanged(nameof(Users));
                OnPropertyChanged(nameof(WaitListedUsers));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Load Screen", $"error has aqured: {ex}", "ok");
            }
            finally { IsRefreshing = false; }
        }
        private async Task OnJoin()
        {
            var s = await proxyloginDemoWebAPI.CheckAsync();
            if (!(s == "User is not logged in!" || s == "FAILED WITH EXCEPTION!"))
            {
                var logedUser = proxyloginDemoWebAPI.LoggedInUser;
                if (!IsJoin)
                {
                    var a = await proxyEventToUser.PostEventAsync(new UserToEvent()
                    {
                        TableId = 0,
                        EventId = selcetedEvent.EventId,
                        UserId = (int)logedUser.UserId,
                        RealtionshipType = "waiting"
                    });
                    if (!a)
                        await Application.Current.MainPage.DisplayAlert("join", "eror", "ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("join", "already joined", "ok");
                }
                IsRefreshing = true;
            }
            else
            {
                var viewModel = new LoginViewModel(proxyloginDemoWebAPI);
                var viewEventPage = new LoginView(viewModel);
                await Shell.Current.Navigation.PushAsync(viewEventPage);
            }
        }
        private async Task OnLeave()
        {
            if (!isJoin)
            {
                await Application.Current.MainPage.DisplayAlert("leave event", $"already not in event", "ok");
                return;
            }
            try
            {
                //var comments = (await proxyChatComment.GetChatCommentsAsync()).Where(u=> u.CommenterId;
                var connection = (await proxyEventToUser.GetEventsAsync()).Where(u => u.EventId == selcetedEvent.EventId && u.UserId == proxyloginDemoWebAPI.LoggedInUser.UserId).First();
                var e = await proxyEventToUser.DeleteEventAsync(connection.TableId);
                if (!e)
                {
                    await Application.Current.MainPage.DisplayAlert("leave event", $"error happend while trying to exit event", "ok");
                    return;
                }
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("leave event", $"error happend while trying to exit event :{ex}", "ok");
                return;
            }
            finally
            {
                IsRefreshing = true;
            }
        }
        public async Task Save()
        {
            await proxyEvent.putEventAsync(SelectedEvent);
            IsRefreshing = true;

        }
        private async Task OnKickUsers()
        {
            var e = new ObservableCollection<Users>(Users.Where(u => u.IsCheck));
            // Logic to remove selected participants
            foreach (var user in e)
            {
                Users.Remove(user);
                await proxyEventToUser.DeleteEventAsync(UserToEvents.Where(e => e.UserId == user.UserId && e.EventId == SelectedEvent.EventId).First().TableId);
            }
            isRefreshing = true;
        }

        private async Task OnWaitlistUsers()
        {
            var e = new ObservableCollection<Users>(Users.Where(u => u.IsCheck));
            // Move selected participants to the waitlist
            foreach (var users in e)
            {
                var userId = users.UserId;
                var user = Users.First(u => u.UserId == userId);
                Users.Remove(user);
                WaitListedUsers.Add(user);
                bool s = await proxyEventToUser.putUserToEventAsync(UserToEvents.Where(e => e.UserId == user.UserId && e.EventId == SelectedEvent.EventId).First().ChangeStatus());
                if(s == false)
                {
                    await Application.Current.MainPage.DisplayAlert("Wait list Users", $"error", "ok");
                    Users.Add(user);
                    waitListedUsers.Remove(user);
                    break;
                }
            }
            isRefreshing = true;
        }

        private async Task OnRejectUsers()
        {
            var e = new ObservableCollection<Users>(WaitListedUsers.Where(u => u.IsCheck));
            // Remove users from the waitlist
            foreach (var users in e)
            {
                var userId = users.UserId;
                WaitListedUsers.Remove(WaitListedUsers.First(u => u.UserId == userId));
                await proxyEventToUser.DeleteEventAsync(UserToEvents.Where(e => e.UserId == userId && e.EventId == SelectedEvent.EventId).First().TableId);
            }
            isRefreshing = true;
        }
        private async Task OnAcceptUsers()
        {
            var e = new ObservableCollection<Users>(WaitListedUsers.Where(u => u.IsCheck));
            // Move waitlisted users to the main participant list
            foreach (var users in e)
            {
                var userId = users.UserId;
                var user = WaitListedUsers.First(u => u.UserId == userId);
                WaitListedUsers.Remove(user);
                Users.Add(user);
                bool s = await proxyEventToUser.putUserToEventAsync(UserToEvents.Where(e => e.UserId == user.UserId && e.EventId == SelectedEvent.EventId).First().ChangeStatus());
                if (s == false)
                {
                    await Application.Current.MainPage.DisplayAlert("Accept Users", $"error", "ok");
                    Users.Remove(user);
                    WaitListedUsers.Add(user);
                    break;
                }
            }
            isRefreshing = true;
        }
        private async Task Comment()
        {
            try
            {
                await proxyloginDemoWebAPI.CheckAsync();
                var logedUser = proxyloginDemoWebAPI.LoggedInUser;
                var chatC = new ChatComment()
                {
                    CreatedAt = DateTime.Now,
                    EventId = SelectedEvent.EventId,
                    CommenterId = logedUser.UserId,
                    CommentText = CommentText,
                };
                var e = await proxyChatComment.PostChatCommentAsync(chatC);
                if (!e)
                    await Application.Current.MainPage.DisplayAlert("post", "error", "ok");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("join", $"error: {ex}", "ok");
            }
            finally { IsRefreshing = true; }
        }
        private async Task DelteComment(ChatComment comment)
        {
            try
            {
                await proxyChatComment.DeleteChatCommentAsync(comment.CommentId);
                IsRefreshing = true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Delete Comment", $"error: {ex}", "ok");
            }

        }
    }
}
